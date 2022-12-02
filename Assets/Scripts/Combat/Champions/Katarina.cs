using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Katarina : ChampionCombat
{
    public static float[] KatarinaPassiveFlatDamageByLevel = { 68, 72, 77, 82, 89, 96, 103, 112, 121, 131, 142, 154, 166, 180, 194, 208, 224, 240 };

    public static float GetKatPassiveAPPercentByLevel(int level)
    {
        return level switch
        {
            < 6 => 65,
            < 11 => 75,
            < 16 => 85,
            _ => 95
        };
    }
    public static float GetKatPassiveECooldownReduction(int level)
    {
        return level switch
        {
            < 6 => 78,
            < 11 => 84,
            < 16 => 90,
            _ => 96
        };
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
        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Physical Damage Per Dagger");  //0
        rKeys.Add("Magic Damage Per Dagger");     //1

        base.UpdatePriorityAndChecks();
    }

    public IEnumerator Voracity(float landingTime)
    {
        yield return new WaitForSeconds(landingTime);
        UpdateTotalDamage(ref pSum, 4, new Damage(KatarinaPassiveFlatDamageByLevel[myStats.level] + (myStats.bonusAD * 0.6f) + (GetKatPassiveAPPercentByLevel(myStats.level) * 0.01f * myStats.AP), SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)16520), "Voracity");
        myStats.eCD -= GetKatPassiveECooldownReduction(myStats.level) * 0.01f * myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 4);
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if(UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)18564) != float.MinValue) StartCoroutine(Voracity(1f));
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), WSkill().basic.name);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        StartCoroutine(Voracity(1.25f));
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)34953);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        attackCooldown = 0.0f;
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateTotalDamage(ref rSum, 3, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), RSkill().basic.name);
        MyBuffManager.Add("Channeling", new ChannelingBuff(2.5f, myStats.buffManager, myStats.rSkill[0].basic.name, "DeathLotus"));
        StartCoroutine(DeathLotus(0));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Katarina's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(2.5f, targetStats.buffManager, myStats.rSkill[0].basic.name, "DeathLotus"));
        StartCoroutine(HDeathLotus(0, skillLevel));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
    }

    public IEnumerator DeathLotus(float time)
    {
        yield return new WaitForSeconds(0.166f);
        time += 0.166f;
        if(UpdateTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)16540), RSkill().basic.name) != float.MinValue)
        {
            UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[1], skillComponentTypes: (SkillComponentTypes)16384);
            if (targetStats.buffManager.buffs.TryGetValue(myStats.rSkill[0].basic.name, out Buff value))
            {
                value.duration = 3f;
            }
            else
            {
                TargetBuffManager.Add(myStats.rSkill[0].basic.name, new GrievousWoundsBuff(3, targetStats.buffManager, myStats.rSkill[0].basic.name, 40f, myStats.rSkill[0].basic.name));
            }
        }
        if (time < 2.5f) StartCoroutine(DeathLotus(time));
    }

    public IEnumerator HDeathLotus(float time, int skillLevel)
    {
        yield return new WaitForSeconds(0.166f);
        time += 0.166f;
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        if (myStats.buffManager.buffs.TryGetValue(myStats.rSkill[0].basic.name, out Buff value))
        {
            value.duration = 3f;
        }
        else
        {
            MyBuffManager.Add(myStats.rSkill[0].basic.name, new GrievousWoundsBuff(3, myStats.buffManager, myStats.rSkill[0].basic.name, 40f, myStats.rSkill[0].basic.name));
        }
        if (time < 2.5f) StartCoroutine(HDeathLotus(time, skillLevel));
    }


    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }
}