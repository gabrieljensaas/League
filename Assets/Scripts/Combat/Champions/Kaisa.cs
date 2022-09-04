using System.Collections;
using UnityEngine;
using Simulator.Combat;

public class Kaisa : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "Q", "A" };
        autoattackcheck = new KaisaAACheck(this, this);
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
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checksR.Add(new CheckIfEnemyHasPlasma(this));
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(0.4f);                                //missle travel time
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4);
    }
    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        if (targetStats.buffManager.buffs.TryGetValue("Plasma", out Buff value))
        {
            value.value += 3;
            if(value.value > 4)
            {
                DealPassiveDamage((targetStats.maxHealth - targetStats.currentHealth) / 100 * (15 + (5 * (myStats.AP % 100))));
                value.value -= 5;
                if (value.value <= 0) value.Kill();
            }
        }
        else
        {
            targetStats.buffManager.buffs.Add("Plasma", new PlasmaBuff(4, targetStats.buffManager, "Kaisa's Auto Attacks"));
        }
        myStats.wCD *= 0.33f;
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(0.5f, myStats.buffManager, myStats.eSkill[0].basic.name));
    }

    public void DealPassiveDamage(float damage)
    {
        UpdateAbilityTotalDamage(ref pSum, 4, damage, myStats.passiveSkill.skillName, SkillDamageType.Spell);
    }
}