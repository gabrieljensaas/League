using System.Collections;
using UnityEngine;
using Simulator.Combat;

public class Jinx : ChampionCombat
{
    private int qStack = 0;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "A", "" };

        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfExecutes(this, "R"));

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
        if (qStack != 3) qStack++;
        myStats.buffManager.buffs.Remove(myStats.qSkill[0].basic.name);
        myStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new AttackSpeedBuff(2.5f, myStats.buffManager, myStats.qSkill[0].basic.name, (myStats.qSkill[0].selfEffects.ASIncreasePercent[4] + (myStats.qSkill[0].selfEffects.ASIncreasePercent[4] * 0.5f * (qStack - 1))) * myStats.baseAttackSpeed, myStats.qSkill[0].basic.name));
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        yield return new WaitForSeconds(0.9f); // chompers landing and arming time
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        targetStats.buffManager.buffs.Add("Root", new RootBuff(1.5f, targetStats.buffManager, myStats.eSkill[0].basic.name));
    }
}