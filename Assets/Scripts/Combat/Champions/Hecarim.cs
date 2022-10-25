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
        qKeys.Add("Minion Damage");
        wKeys.Add("Magic Damage Per Tick");
        wKeys.Add("Total Magic Damage");
        wKeys.Add("Bonus Resistances");
        wKeys.Add("Capped Healing");
        eKeys.Add("Minimum Physical Damage");
        eKeys.Add("Maximum Physical Damage");
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
        SpiritOfTheDread(AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal)).damage);
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        StopCoroutine(LoseRampageStacks());
        SpiritOfTheDread(UpdateTotalDamage(ref qSum, 0, new Damage(myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats) + (rampageStacks * 0.04f * myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats)), SkillDamageType.Phyiscal), myStats.qSkill[0].name)); //no bonus AD for now
        if (rampageStacks < 3) rampageStacks++;
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        myStats.qCD -= rampageStacks * 0.75f;
        StartCoroutine(LoseRampageStacks());
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("BonusArmor", new ArmorBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, (int)myStats.wSkill[0].UseSkill(4, wKeys[2], myStats, targetStats), "BonusArmor"));
        myStats.buffManager.buffs.Add("BonusMR", new MagicResistanceBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, (int)myStats.wSkill[0].UseSkill(4, wKeys[2], myStats, targetStats), "BonusMR"));
        myStats.buffManager.buffs.Add("SpiritOfTheDread", new SpiritOfTheDreadBuff(4, myStats.buffManager, myStats.eSkill[0].name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        SpiritOfTheDread(UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0])); //does not account for movement as of the moment
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.25f, targetStats.buffManager, myStats.eSkill[0].name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        SpiritOfTheDread(UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]));
        targetStats.buffManager.buffs.Add("Flee", new FleeBuff(0.75f, targetStats.buffManager, myStats.eSkill[0].name));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
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

    public void SpiritOfTheDread(float damage)
    {
        if (myStats.buffManager.buffs.ContainsKey("SpiritOfTheDread"))
            UpdateTotalHeal(ref hSum, damage * 0.25f, myStats.wSkill[0].basic.name);
    }
}
