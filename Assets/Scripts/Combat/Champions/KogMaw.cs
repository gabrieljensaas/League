using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class KogMaw : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "E", "Q", "A" };

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
        autoattackcheck = new KogMawAACheck(this);

        //add q passive bonus here

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        targetStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new ArmorReductionBuff(4, targetStats.buffManager, myStats.qSkill[0].basic.name, Constants.KogMawQReductionBySkillLevel[4], myStats.qSkill[0].basic.name));
        targetStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new MagicResistanceReductionBuff(4, targetStats.buffManager, myStats.qSkill[0].basic.name, Constants.KogMawQReductionBySkillLevel[4], myStats.qSkill[0].basic.name));
    }
    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        myStats.buffManager.buffs.Add("BioArcaneBarrage", new BioArcaneBarrageBuff(8, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, myStats, targetStats, wKeys)));
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        yield return new WaitForSeconds(0.6f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys);
    }
}