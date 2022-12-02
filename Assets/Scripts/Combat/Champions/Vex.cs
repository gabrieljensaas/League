using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Vex : ChampionCombat
{
    public float gloomTimer = 0;

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        gloomTimer -= Time.fixedDeltaTime;
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "Q", "W", "R", "A" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));

        checksQ.Add(new CheckIfCasting(this));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));

        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));

        checksR.Add(new CheckIfImmobilize(this));

        checkTakeDamagePostMitigation.Add(new CheckShield(this));
        checkTakeDamage.Add(new CheckIfEnemyDashedOrBlinked(this));

        qKeys.Add("Magic Damage");
        wKeys.Add("Shield Strength");
        wKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if(UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)18564) != float.MinValue) Doom(myStats.qSkill[0].name);
        Gloom();
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        if(UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[1], skillComponentTypes: (SkillComponentTypes)18560) != float.MinValue) Doom(myStats.wSkill[0].name);
        Gloom();
        myStats.buffManager.shields.Add(myStats.wSkill[0].basic.name, new ShieldBuff(2.5f, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), myStats.wSkill[0].basic.name));
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        if (UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)18564) != float.MinValue)
        {
            Gloom();
            TargetBuffManager.Add("Gloom", new GloomBuff(6, TargetBuffManager));
            Doom(myStats.eSkill[0].name);
        }
        else Gloom();
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        if(UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)34948) != float.MinValue)
        {
            Gloom();
            Doom(myStats.rSkill[0].name);
            if(UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[1]) != float.MinValue) Doom(myStats.rSkill[0].name);
            Gloom();
        }
        else Gloom();
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (TargetBuffManager.buffs.TryGetValue("Gloom", out Buff value))
        {
            value.Kill();
            UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Vex' Auto Attack");
            UpdateTotalDamage(ref pSum, 4, new Damage(30 + (110 / 17 * (myStats.level - 1)), SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)1824), "Doom 'n Gloom");
        }
        else UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Vex' Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalDamage(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[4] * 2;
    }

    private void Gloom()
    {
        if(TargetBuffManager.buffs.TryGetValue("Gloom", out Buff value))
        {
            value.Kill();
            UpdateTotalDamage(ref pSum, 4, new Damage(30 + (110 / 17 * (myStats.level - 1)), SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)32), "Doom 'n Gloom");
        }
    }

    private void Doom(string skillName)
    {
        if (gloomTimer <= 0)
        {
            TargetBuffManager.Add("Flee", new FleeBuff(GetVexDoomFear(myStats.level), targetStats.buffManager, skillName));
            gloomTimer = GetVexDoomCooldown(myStats.level);
        }
    }

    public static float GetVexDoomCooldown(int level)
    {
        return level switch
        {
            < 6 => 25,
            < 11 => 22,
            < 16 => 19,
            _ => 16
        };
    }

    public static float GetVexDoomFear(int level)
    {
        return level switch
        {
            < 6 => 0.75f,
            < 9 => 1f,
            < 13 => 1.25f,
            _ => 1.5f
        };
    }
}
