using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Hecarim : ChampionCombat
{
    private int rampageStacks;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "E", "R", "W", "A" };

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

        qKeys.Add("Physical Damage");
        wKeys.Add("Magic Damage Per Tick");
        wKeys.Add("Bonus Resistances");
        eKeys.Add("Minimum Physical Damage");
        rKeys.Add("Magic damage");

        base.UpdatePriorityAndChecks();
    }

    /// <summary>
    /// NO BONUS MOVEMENT SPEED AT THE MOMENT SO PASSIVE ISN'T WORKING
    /// </summary>
    /// <returns></returns>

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        SpiritOfTheDread(UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Hecarim's Auto Attack"));
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        StopCoroutine(LoseRampageStacks());
        SpiritOfTheDread(UpdateTotalDamage(ref qSum, 0, new Damage(myStats.qSkill[0].UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats) + (rampageStacks * 0.04f * myStats.qSkill[0].UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats)), SkillDamageType.Phyiscal, skillComponentType:(SkillComponentTypes)18560), myStats.qSkill[0].name)); //no bonus AD for now
        if (rampageStacks < 3) rampageStacks++;
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        myStats.qCD -= rampageStacks * 0.75f;
        simulationManager.AddCastLog(myCastLog, 0);
        StartCoroutine(LoseRampageStacks());
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), WSkill().basic.name);
        MyBuffManager.Add("BonusArmor", new ArmorBuff(4, MyBuffManager, myStats.wSkill[0].basic.name, (int)myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), "BonusArmor"));
        MyBuffManager.Add("BonusMR", new MagicResistanceBuff(4, MyBuffManager, myStats.wSkill[0].basic.name, (int)myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), "BonusMR"));
        MyBuffManager.Add("SpiritOfTheDread", new SpiritOfTheDreadBuff(4, MyBuffManager, myStats.eSkill[0].name));
        StartCoroutine(SpiritOfDread());
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        SpiritOfTheDread(UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], skillComponentTypes:(SkillComponentTypes)39808)); //does not account for movement as of the moment
        TargetBuffManager.Add("Stun", new StunBuff(0.25f, TargetBuffManager, myStats.eSkill[0].name));
        attackCooldown = 0;
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        SpiritOfTheDread(UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes:(SkillComponentTypes)18560));
        TargetBuffManager.Add("Flee", new FleeBuff(0.75f, TargetBuffManager, myStats.rSkill[0].name));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 4);
    }

    private IEnumerator LoseRampageStacks()
    {
        yield return new WaitForSeconds(6);

        while (rampageStacks > 0)
        {
            rampageStacks--;
            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator SpiritOfDread()
	{
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)8192);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)8192);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)8192);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)8192);
    }

    public void SpiritOfTheDread(float damage)
    {
        if (MyBuffManager.buffs.ContainsKey("SpiritOfTheDread"))
            UpdateTotalHeal(ref hSum, damage * 0.25f, WSkill().basic.name);
    }
}
