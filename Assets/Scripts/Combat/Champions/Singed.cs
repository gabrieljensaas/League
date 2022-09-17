using Simulator.Combat;
using System.Collections;

public class Singed : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        checksA.Add(new CheckIfTotalCC(this));
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Magic Damage per Tick");
        eKeys.Add("Magic Damage");
        rKeys.Add("Bonus Stats");
        rKeys.Add("Regeneration per Second");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));

        targetStats.buffManager.buffs.Add("Poison Trail", new PoisonTrailBuff(3, targetStats.buffManager, myStats.qSkill[0].basic.name));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("BonusAP", new AbilityPowerBuff(25, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), "BonusAP"));
        myStats.buffManager.buffs.Add("BonusArmor", new ArmorBuff(25, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), "BonusArmor"));
        myStats.buffManager.buffs.Add("BonusMR", new MagicResistanceBuff (25, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), "BonusMR"));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

}