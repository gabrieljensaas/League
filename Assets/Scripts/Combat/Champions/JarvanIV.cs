using Simulator.Combat;
using System.Collections;

public class JarvanIV : ChampionCombat
{
    public static float MartialCadenceTargetImmunity(int level)
    {
        return level switch
        {
            < 6 => 6,
            < 11 => 5,
            < 16 => 4,
            _ => 3
        };
    }

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

        checkTakeDamagePostMitigation.Add(new CheckShield(this));
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Physical Damage");
        qKeys.Add("Armor Reduction");
        wKeys.Add("Slow");
        wKeys.Add("Base Shield Strength");
        eKeys.Add("Bonus Attack Speed");
        eKeys.Add("Magic Damage");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();

        myStats.buffManager.buffs.Add("DemacianStandardBonus", new AttackSpeedBuff(999f, myStats.buffManager, myStats.eSkill[0].name, myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats), "DemacianStandardBonus"));
    }

    public override IEnumerator ExecuteA()
    {
        yield return base.ExecuteA();
        if (targetStats.buffManager.buffs.TryAdd("MartialCadence", new MartialCadenceBuff(MartialCadenceTargetImmunity(myStats.level), targetStats.buffManager, myStats.qSkill[0].name)))
            UpdateTotalDamage(ref pSum, 4, new Damage(targetStats.maxHealth * 0.08f, SkillDamageType.Phyiscal), myStats.passiveSkill.skillName);
    }

    public override IEnumerator ExecuteQ()
    {
        yield return base.ExecuteQ();
        targetStats.buffManager.buffs.Add("ArmorReduction", new ArmorReductionBuff(3, targetStats.buffManager, myStats.qSkill[0].name, myStats.qSkill[0].UseSkill(4, qKeys[1], myStats, targetStats), "ArmorReduction"));
        if (myStats.buffManager.buffs.ContainsKey("DemacianStandardFlag"))
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.75f, targetStats.buffManager, myStats.qSkill[0].name));
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.shields.Add(myStats.wSkill[0].basic.name, new ShieldBuff(5, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
        myStats.buffManager.buffs.Add("DemacianStandardAS", new AttackSpeedBuff(8, myStats.buffManager, myStats.eSkill[0].name, myStats.eSkill[0].UseSkill(4, eKeys[1], myStats, targetStats), "DemacianStandardAS"));
        myStats.buffManager.buffs.Add("DemacianStandardFlag", new DemacianStandardBuff(8, myStats.buffManager, myStats.eSkill[0].name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        yield return base.ExecuteR();
        myStats.buffManager.buffs.Add("DisplacementImmunity", new DisplacementImmunityBuff(0.35f, myStats.buffManager, "DisplacementImmunity"));
    }
}
