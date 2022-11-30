using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Mordekaiser : ChampionCombat
{
    private float shieldStored;
    public static float DarknessRiseDamage(int level) => 4.4f + (0.6f * level);
    public static float DarknessRiseTargetMaxHPPercentDamage(int level) => 0.01f + (0.04f / 17 * (level - 1));

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "A", "W" };

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
        checksA.Add(new CheckIfDisarmed(this));

        checkTakeDamage.Add(new CheckMordekaiserIndestructible(this, this));
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Damage Increase");
        wKeys.Add("Shield to Healing");
        eKeys.Add("Magic Penetration");
        eKeys.Add("Magic Damage");

        targetStats.spellBlock *= (100 - ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats)) * 0.01f;
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        Indestructible(UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Mordekaiser's Auto Attack"));
        Indestructible(UpdateTotalDamage(ref pSum, 4, new Damage(0.4f * myStats.AP, SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)5912), myStats.passiveSkill.skillName));
        DarknessRiseStacks(myStats.passiveSkill.skillName);
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        Indestructible(UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)34944));
        Indestructible(UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)34944));
        DarknessRiseStacks(QSkill().name);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    //decay is unspecified in fandom
    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        MyBuffManager.shields.Add(WSkill().basic.name, new ShieldBuff(5, MyBuffManager, WSkill().basic.name, shieldStored, WSkill().basic.name));
        shieldStored = 0;
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);

        StartCoroutine(ConsumeShield());
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        Indestructible(UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[1], skillComponentTypes:(SkillComponentTypes)18560));
        DarknessRiseStacks(ESkill().name);
        TargetBuffManager.Add("Airborne", new AirborneBuff(0.1f, TargetBuffManager, ESkill().name)); //pull
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateTotalHeal(ref hSum, (targetStats.maxHealth * 0.1f), RSkill().name);
        StartCoroutine(StealStats());
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 4);
    }

    private void DarknessRiseStacks(string source)
    {
        if (MyBuffManager.buffs.TryGetValue("DarknessRise", out Buff buff))
        {
            buff.duration = 4;
            if (buff.value < 3) buff.value++;
        }
        else
            MyBuffManager.Add("DarknessRise", new DarknessRiseBuff(4, MyBuffManager, source, this));
    }

    private IEnumerator StealStats()
    {
        myStats.AD += targetStats.AD * 0.1f;
        myStats.AP += targetStats.AP * 0.1f;
        myStats.attackSpeed += targetStats.attackSpeed * 0.1f;
        myStats.armor += targetStats.armor * 0.1f;
        myStats.spellBlock += targetStats.spellBlock * 0.1f;

        targetStats.AD *= 0.9f;
        targetStats.AP *= 0.9f;
        targetStats.attackSpeed *= 0.9f;
        targetStats.armor *= 0.9f;
        targetStats.spellBlock *= 0.9f;

        yield return new WaitForSeconds(7f);

        myStats.AD -= targetStats.AD * 0.1f;
        myStats.AP -= targetStats.AP * 0.1f;
        myStats.attackSpeed -= targetStats.attackSpeed * 0.1f;
        myStats.armor -= targetStats.armor * 0.1f;
        myStats.spellBlock -= targetStats.spellBlock * 0.1f;

        targetStats.AD /= 0.9f;
        targetStats.AP /= 0.9f;
        targetStats.attackSpeed /= 0.9f;
        targetStats.armor /= 0.9f;
        targetStats.spellBlock /= 0.9f;
    }

    //did not add decay because champion wil always be in combat anyways
    public void Indestructible(float damage)
    {
        shieldStored += damage;

        if (shieldStored > myStats.maxHealth * 0.3f) shieldStored = myStats.maxHealth * 0.3f;
    }

    private IEnumerator ConsumeShield()
    {
        yield return new WaitForSeconds(4);
        if (MyBuffManager.buffs.TryGetValue(WSkill().basic.name, out Buff buff))
        {
            UpdateTotalHeal(ref hSum, buff.value * WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * 0.01f, WSkill().name);
            buff.Kill();
        }
    }
}
