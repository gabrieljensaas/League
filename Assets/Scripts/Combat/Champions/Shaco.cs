using Simulator.Combat;
using System.Collections;

public class Shaco : ChampionCombat
{
    public static float JackInTheBoxHP(int level)
    {
        return level switch
        {
            < 4 => 150,
            < 10 => 150 + ((level - 3) * 10),
            < 12 => 210 + ((level - 9) * 20),
            < 14 => 250 + ((level - 11) * 25),
            _ => 300 + ((level - 13) * 50)
        };
    }

    public static float JackInTheBoxHallucinationHP(int level)
    {
        return level switch
        {
            < 10 => 340 + (level - 5) * 20,
            < 12 => 420 + (level - 9) * 40,
            < 14 => 500 + (level - 11) * 50,
            _ => 600 + (level - 13) * 100
        };
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "W", "R", "A", "E" };

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

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Invisibility Duration");
        qKeys.Add("Bonus Physical Damage");
        wKeys.Add("Champion Disable Duration");
        wKeys.Add("Magic Damage");
        wKeys.Add("Increased Damage");
        eKeys.Add("Slow");
        eKeys.Add("Magic Damage");
        eKeys.Add("Increased Damage");
        eKeys.Add("Slow");
        rKeys.Add("Magic damage");
        rKeys.Add("Modified Magic Damage");
        rKeys.Add("Increased Modified Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Shaco's Auto Attack") != float.MinValue);
		{
            if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff buff))
            {
                UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes:(SkillComponentTypes)5016);
                buff.Kill();
            }
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), QSkill().basic.name);
        MyBuffManager.Add("Untargetable", new UntargetableBuff(myStats.qSkill[0].UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), myStats.buffManager, myStats.qSkill[0].basic.name));
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        pets.Add(new JackInTheBox(this, JackInTheBoxHP(myStats.level), myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[2], myStats, targetStats), 2, 100, 50, 5, 2));
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));

        if (myStats.buffManager.buffs.TryGetValue("Untargetable", out Buff buff))
            buff.Kill();

        if (targetStats.PercentCurrentHealth > 0.3f)
            UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[1], skillComponentTypes:(SkillComponentTypes)34820);
        else
            UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[2], skillComponentTypes: (SkillComponentTypes)34820);

        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        pets.Add(new Hallucination(this, myStats.currentHealth, myStats.AD, myStats.attackSpeed, myStats.spellBlock, myStats.armor, 18));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 4);
    }
}
