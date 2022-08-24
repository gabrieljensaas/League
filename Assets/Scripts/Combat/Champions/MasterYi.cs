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
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill, 4);
        myStats.qCD = myStats.qSkill.basic.coolDown[4];
    }
}