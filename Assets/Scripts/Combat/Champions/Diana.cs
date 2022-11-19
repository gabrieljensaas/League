using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Diana : ChampionCombat
{
    public static float MoonsilverBladeAS(int level)
    {
        return level switch
        {
            < 3 => 0.15f,
            < 6 => 0.1917f,
            < 9 => 0.275f,
            < 12 => 0.3167f,
            < 15 => 0.3583f,
            _ => 0.4f
        };
    }

    public static float MoonsilverBladeCleave(int level)
    {
        return level switch
        {
            < 7 => 20 + 5 * (level - 1),
            < 12 => 45 + 10 * (level - 6),
            < 16 => 95 + 15 * (level - 11),
            _ => 155 + 25 * (level - 15)
        };
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "E", "W", "R", "A" };

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

        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        wKeys.Add("Total Shield Strength");
        wKeys.Add("Total Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();

        MyBuffManager.Add("MoonsilverAS", new AttackSpeedBuff(float.MaxValue, MyBuffManager, "Moonsilver Blade", MoonsilverBladeAS(myStats.level), "MoonsilverAS"));
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Diana's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
        MoonsilverBladeAA();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)18564);
        TargetBuffManager.Add("Moonlight", new MoonlightBuff(3, TargetBuffManager, QSkill().name));
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
        MoonsilverBladeBonusAS();
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[1], skillComponentTypes: (SkillComponentTypes)18560);
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
        MyBuffManager.Add("PaleCascade", new ShieldBuff(5, MyBuffManager, WSkill().name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), "PaleCascade"));
        MoonsilverBladeBonusAS();
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)34946);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
        if (TargetBuffManager.buffs.TryGetValue("Moonlight", out Buff buff))
        {
            buff.Kill();
            myStats.eCD = 0.5f;
        }
        MoonsilverBladeBonusAS();
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        TargetBuffManager.Add("Airborne", new AirborneBuff(0.25f, TargetBuffManager, myStats.rSkill[0].name));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];

        yield return new WaitForSeconds(1);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[1], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)18560);
        MoonsilverBladeBonusAS();
    }

    private void MoonsilverBladeAA()
    {
        if (MyBuffManager.buffs.TryGetValue("MoonsilverBlade", out Buff buff))
        {
            if (++buff.value == 2)
            {
                buff.Kill();
                UpdateTotalDamage(ref pSum, 4, new Damage(MoonsilverBladeCleave(myStats.level) + (myStats.AP * 0.5f), SkillDamageType.Spell, (SkillComponentTypes)17048), myStats.passiveSkill.skillName);
            }
        }
        else
            MyBuffManager.Add("MoonsilverBlade", new MoonsilverBladeBuff(3.5f, MyBuffManager, myStats.passiveSkill.skillName));
    }

    private void MoonsilverBladeBonusAS()
    {
        StopCoroutine(ReturnToNormalMoonsilverAS());
        if (MyBuffManager.buffs.TryGetValue("MoonsilverAS", out Buff AS))
            AS.Kill();
        else if (MyBuffManager.buffs.TryGetValue("MoonsilverBonusAS", out Buff bonusAS))
            bonusAS.duration = 3;

        MyBuffManager.Add("MoonsilverBonusAS", new AttackSpeedBuff(3, MyBuffManager, "Moonsilver Blade Bonus AS", MoonsilverBladeAS(myStats.level) * 3, "MoonsilverBonusAS"));
        StartCoroutine(ReturnToNormalMoonsilverAS());
    }

    private IEnumerator ReturnToNormalMoonsilverAS()
    {
        yield return new WaitForSeconds(3);
        MyBuffManager.Add("MoonsilverAS", new AttackSpeedBuff(float.MaxValue, MyBuffManager, "Moonsilver Blade", MoonsilverBladeAS(myStats.level), "MoonsilverAS"));
    }
}
