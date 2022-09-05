using System.Collections;
using UnityEngine;
using Simulator.Combat;
using System.Collections.Generic;

public class Kalista : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "Q", "A", "", "" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksE.Add(new CheckCD(this, "E"));
        checksA.Add(new CheckCD(this, "A"));
        checksQ.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfEnemyHasRend(this));
        checksE.Add(new CheckIfExecutes(this, "E"));
        checksA.Add(new CheckIfCasting(this));

        base.UpdatePriorityAndChecks();
    }
    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        if (targetStats.buffManager.buffs.TryGetValue("Rend", out Buff value))
        {
            value.value++;
            value.duration = 4;
        }
        else
        {
            targetStats.buffManager.buffs.Add("Rend", new RendBuff(4, targetStats.buffManager, myStats.qSkill[0].basic.name));
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        targetStats.buffManager.buffs.TryGetValue("Rend", out Buff value);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, ((value.value - 1) * 0.5f) + 1);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(0.9f);
        if (targetStats.buffManager.buffs.TryGetValue("Rend", out Buff value))
        {
            value.value++;
            value.duration = 4;
        }
        else
        {
            targetStats.buffManager.buffs.Add("Rend", new RendBuff(4, targetStats.buffManager, myStats.qSkill[0].basic.name));
        }
    }
}