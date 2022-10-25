using Simulator.Combat;
using System.Collections;

public class Trundle : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "W", "E", "A" };

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
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        autoattackcheck = new TrundleAACheck(this);

        qKeys.Add("Bonus Physical Damage");
        qKeys.Add("Bonus Attack Damage");
        qKeys.Add("Attack Damage Reduction");
        wKeys.Add("Bonus Attack Speed");
        rKeys.Add("Total Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Chomp", new ChompBuff(7, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats)));
        myStats.buffManager.buffs.Add("BonusAD", new AttackDamageBuff(5, myStats.buffManager, myStats.qSkill[0].basic.name, (int)myStats.qSkill[0].UseSkill(4, qKeys[1], myStats, targetStats), "BonusAD"));
        targetStats.buffManager.buffs.Add("ADReduction", new AttackDamageReductionBuff(5, targetStats.buffManager, myStats.qSkill[0].basic.name, (int)myStats.qSkill[0].UseSkill(4, qKeys[2], myStats, targetStats), "ADReduction"));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        attackCooldown = 0;
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("AttackSpeedBuff", new AttackSpeedBuff(8, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), "AttackSpeedBuff"));
        myStats.buffManager.buffs.Add("IncreasedHeal", new GrievousWoundsBuff(8, myStats.buffManager, myStats.wSkill[0].basic.name, -25f, "IncreasedHeal"));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("KnockBackBuff", new AirborneBuff(0.1f, targetStats.buffManager, "KnockBackBuff"));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);        //need to split half of the damage to time
        UpdateTotalHeal(ref rSum, myStats.rSkill[0], 2, rKeys[0]);
        myStats.buffManager.buffs.Add("BonusArmor", new ArmorBuff(8, myStats.buffManager, myStats.rSkill[0].basic.name, (float)0.4 * targetStats.armor, "BonusArmor"));
        myStats.buffManager.buffs.Add("ArmorReduction", new ArmorReductionBuff(8, targetStats.buffManager, myStats.qSkill[0].basic.name, (float)0.4 * targetStats.armor, "ArmorReduction"));
        myStats.buffManager.buffs.Add("BonusMR", new MagicResistanceBuff(8, myStats.buffManager, myStats.qSkill[0].basic.name, (int)(0.4 * targetStats.spellBlock), "BonusMR"));
        myStats.buffManager.buffs.Add("MRReduction", new MagicResistanceReductionBuff(8, myStats.buffManager, myStats.qSkill[0].basic.name, (float)0.4 * targetStats.spellBlock, "MRReduction"));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }
}