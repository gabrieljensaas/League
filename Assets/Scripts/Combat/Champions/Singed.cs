using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Singed : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "", "R", "A" };

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

        qKeys.Add("Magic Damage per Tick");
        eKeys.Add("Magic Damage");
        eKeys.Add("Root Duration");
        rKeys.Add("Bonus Stats");
        rKeys.Add("Regeneration per Second");

        base.UpdatePriorityAndChecks();

        targetStats.buffManager.buffs.Add("Poison Trail", new PoisonTrailBuff(float.MaxValue, targetStats.buffManager, myStats.qSkill[0].basic.name));
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Ground", new GroundBuff(3, targetStats.buffManager, myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.25f, targetStats.buffManager, myStats.wSkill[0].basic.name));  //duration is a guess needs research
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(0.25f);
        if (targetStats.buffManager.buffs.ContainsKey("Ground"))
        {
            targetStats.buffManager.buffs.Add("Root", new RootBuff(myStats.eSkill[0].UseSkill(4, eKeys[1], myStats, targetStats), targetStats.buffManager, myStats.eSkill[0].basic.name));
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("BonusAP", new AbilityPowerBuff(25, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), "BonusAP"));
        myStats.buffManager.buffs.Add("BonusArmor", new ArmorBuff(25, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), "BonusArmor"));
        myStats.buffManager.buffs.Add("BonusMR", new MagicResistanceBuff(25, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), "BonusMR"));
        targetStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new GrievousWoundsBuff(26, targetStats.buffManager, myStats.rSkill[0].basic.name, 25, myStats.rSkill[0].basic.name));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("BonusAP", new AbilityPowerBuff(25, targetStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(skillLevel, rKeys[0], targetStats, myStats), "BonusAP"));
        targetStats.buffManager.buffs.Add("BonusArmor", new ArmorBuff(25, targetStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(skillLevel, rKeys[0], targetStats, myStats), "BonusArmor"));
        targetStats.buffManager.buffs.Add("BonusMR", new MagicResistanceBuff(25, targetStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(skillLevel, rKeys[0], targetStats, myStats), "BonusMR"));
        myStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new GrievousWoundsBuff(26, myStats.buffManager, myStats.rSkill[0].basic.name, 25, myStats.rSkill[0].basic.name));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
    }
}