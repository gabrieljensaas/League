using System.Collections;
using UnityEngine;
using Simulator.Combat;
using System.Collections.Generic;
using TMPro;

public class Draven : ChampionCombat
{
    private int pStack = 1;
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
        checksR.Add(new CheckIfExecutes(this, "R", pStack, 2));
        checksA.Add(new CheckIfCasting(this));
        targetCombat.checksQ.Add(new CheckIfAirborne(targetCombat));
        targetCombat.checksW.Add(new CheckIfAirborne(targetCombat));
        targetCombat.checksE.Add(new CheckIfAirborne(targetCombat));
        targetCombat.checksR.Add(new CheckIfAirborne(targetCombat));
        targetCombat.checksA.Add(new CheckIfAirborne(targetCombat));

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if(myStats.buffManager.buffs.TryGetValue("SpinningAxe", out Buff value))
        {
            if (value.value != 2) value.value++;
            value.duration = 5.8f;
        }
        else
        {
            myStats.buffManager.buffs.Add("SpinningAxe", new SpinningAxeBuff(5.8f, myStats.buffManager, myStats.qSkill[0].basic.name));
        }
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.1f, targetStats.buffManager, myStats.eSkill[0].basic.name));
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2);
        if (targetStats.currentHealth <= pStack) targetStats.currentHealth -= pStack;
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2);
        if (targetStats.currentHealth <= pStack) targetStats.currentHealth -= pStack;
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
        if(myStats.buffManager.buffs.TryGetValue("SpinningAxe", out Buff value))
        {
            value.value--;
            StartCoroutine(SpinnigAxe());
            if (value.value == 0) value.Kill();
        }

    }

    private IEnumerator SpinnigAxe()
    {
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4);
        yield return new WaitForSeconds(2);
        myStats.wCD = -0.1f;
        if (myStats.buffManager.buffs.TryGetValue("SpinningAxe", out Buff value))
        {
            value.value++;
            value.duration = 5.8f;
        }
        else
        {
            myStats.buffManager.buffs.Add("SpinningAxe", new SpinningAxeBuff(5.8f, myStats.buffManager, myStats.qSkill[0].basic.name));
        }

    }
}