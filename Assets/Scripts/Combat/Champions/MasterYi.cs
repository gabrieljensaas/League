using Simulator.Combat;
using System.Collections;
using UnityEngine;
public class MasterYi : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "W", "A" };

        checksQ.Add(new CheckIfCasting(this));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        checksQ.Add(new CheckIfUnableToAct(this));
        checksW.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));
        autoattackcheck = new MasterYiAACheck(this);
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checkTakeDamage.Add(new CheckIfTargetable(this));
        checkTakeDamageAA.Add(new CheckIfTargetable(this));
        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill.basic.castTime));
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(1.089f, myStats.buffManager, myStats.qSkill.basic.name));
        myStats.buffManager.buffs.Add("UnableToAct", new UnableToActBuff(1.089f, myStats.buffManager, myStats.qSkill.basic.name));
        myStats.qCD = myStats.qSkill.basic.coolDown[4];

        yield return new WaitForSeconds(0.231f);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill, 4, 0.25f);
        //update wuju style and highlander


        yield return new WaitForSeconds(0.231f);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill, 4, 0.25f);

        yield return new WaitForSeconds(0.231f);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill, 4, 0.25f);

        yield return new WaitForSeconds(0.165f);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill, 4);

    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill.basic.castTime));
        
        
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill, 4);


        myStats.wCD = myStats.wSkill.basic.coolDown[4];
    }

    private IEnumerator Meditate()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill, 4);

    }
}