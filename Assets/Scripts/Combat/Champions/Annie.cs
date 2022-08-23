using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulator.Combat;
using static AttributeTypes;
using Unity.VisualScripting;

public class Annie : ChampionCombat
{
    private CheckAnnieP annieP;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "Q", "A" };
        annieP = new CheckAnnieP(this);
        checksQ.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckQCD(this));
        targetCombat.checksQ.Add(new CheckIfStunned(targetCombat));
        checksW.Add(new CheckIfCasting(this));
        checksW.Add(new CheckWCD(this));
        targetCombat.checksW.Add(new CheckIfStunned(targetCombat));
        checksE.Add(new CheckIfCasting(this));
        checksE.Add(new CheckECD(this));
        targetCombat.checksE.Add(new CheckIfStunned(targetCombat));
        checksR.Add(new CheckIfCasting(this));
        checksR.Add(new CheckRCD(this));
        targetCombat.checksR.Add(new CheckIfStunned(targetCombat));
        checksA.Add(new CheckIfCasting(this));
        checksA.Add(new CheckACD(this));
        targetCombat.checksA.Add(new CheckIfStunned(targetCombat));
        checkTakeDamage.Add(new CheckShield(this));
        checkTakeDamageAA.Add(new CheckMoltenShield(this));
        checkTakeDamageAA.Add(new CheckShield(this));
        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        foreach (var item in pets)
        {
            item.Update();
        }
    }

    protected override void CheckDeath()
    {
        if (myStats.currentHealth <= 0 && pets.Count > 0)
            StartCoroutine(TibbersRevenge());
        else if (myStats.currentHealth <= 0)
            EndBattle();
    }

    private IEnumerator TibbersRevenge()
    {
        Tibbers tibbers = (Tibbers)pets[0];
        tibbers.Enrage(10);
        yield return new WaitForSeconds(10);
        EndBattle();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill.basic.castTime));
        CheckAnniePassiveStun(myStats.qSkill.basic.name);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill, 4);
        myStats.qCD = myStats.qSkill.basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill.basic.castTime));
        CheckAnniePassiveStun(myStats.wSkill.basic.name);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill, 4);
        myStats.wCD = myStats.wSkill.basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill.basic.castTime));
        if (myStats.buffManager.buffs.TryGetValue("Pyromania", out Buff buff) && buff.value < 4)
        {
            buff.value++;
            simulationManager.ShowText($"{myStats.name} Gained A Stack of Pyromania From {myStats.eSkill.basic.name}");
        }
        else
        {
            myStats.buffManager.buffs.Add("Pyromania", new PyromaniaBuff(myStats.buffManager, myStats.eSkill.basic.name));
        }
        //UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill, 4);
        myStats.eSkill.UseSkill(4, myStats, targetStats);
        myStats.eCD = myStats.eSkill.basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill.basic.castTime));
        CheckAnniePassiveStun(myStats.rSkill.basic.name);
        UpdateAbilityTotalDamage(ref qSum, 3, myStats.rSkill, 2);
        myStats.rCD = myStats.rSkill.basic.coolDown[2];
        pets.Add(new Tibbers(this, 3100, 100 + (myStats.AP * 15 / 100), 0.625f, 90, 90)); //all stats are for max level change when level adjusting of skills done
    }

    private void CheckAnniePassiveStun(string skillName)
    {
        if (annieP.Control())
        {
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(Constants.GetAnnieStunDurationByLevel(myStats.level), targetStats.buffManager, myStats.passiveSkill.skillName));
            myStats.buffManager.buffs.Remove("Pyromania");
        }
        else if (myStats.buffManager.buffs.TryGetValue("Pyromania", out Buff pyromania))
        {
            pyromania.value++;
            simulationManager.ShowText($"{myStats.name} Gained A Stack of Pyromania From {skillName}");
        }
        else
        {
            myStats.buffManager.buffs.Add("Pyromania", new PyromaniaBuff(myStats.buffManager, skillName));
        }
    }
}