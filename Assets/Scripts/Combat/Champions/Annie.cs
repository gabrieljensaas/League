using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulator.Combat;
using static AttributeTypes;

public class Annie : ChampionCombat
{
    private CheckAnnieP annieP;
    public override void UpdatePriorityAndChecks()
    {
        base.UpdatePriorityAndChecks();
        combatPrio[0] = "R";
        combatPrio[1] = "E";
        combatPrio[2] = "W";
        combatPrio[3] = "Q";
        combatPrio[4] = "A";
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
        myUI.combatPriority.text = string.Join(", ", combatPrio);
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill.basic.castTime));
        if (annieP.Control())
        {
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(Constants.GetAnnieStunDurationByLevel(myStats.level), targetStats.buffManager, myStats.passiveSkill.skillName));
            myStats.buffManager.buffs.Remove("Pyromania");
        }
        else
        {
            if (myStats.buffManager.buffs.TryGetValue("Pyromania", out Buff value))
            {
                value.value++;
                simulationManager.ShowText($"{myStats.name} Gained A Stack of Pyromania From {myStats.qSkill.basic.name}");
            }
            else
            {
                myStats.buffManager.buffs.Add("Pyromania", new PyromaniaBuff(myStats.buffManager, myStats.qSkill.basic.name));
            }
        }
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill, 4);
        myStats.qCD = myStats.qSkill.basic.coolDown[4];
    }
    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill.basic.castTime));
        if (annieP.Control())
        {
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(Constants.GetAnnieStunDurationByLevel(myStats.level), targetStats.buffManager, myStats.passiveSkill.skillName));
            myStats.buffManager.buffs.Remove("Pyromania");
        }
        else
        {
            if (myStats.buffManager.buffs.TryGetValue("Pyromania", out Buff pyromania))
            {
                pyromania.value++;
                simulationManager.ShowText($"{myStats.name} Gained A Stack of Pyromania From {myStats.wSkill.basic.name}");
            }
            else
            {
                myStats.buffManager.buffs.Add("Pyromania", new PyromaniaBuff(myStats.buffManager, myStats.wSkill.basic.name));
            }
        }
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill, 4);
        myStats.wCD = myStats.wSkill.basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill.basic.castTime));
        if (myStats.buffManager.buffs.TryGetValue("Pyromania", out Buff buff))
        {
            if (buff.value < 4)
            {
                buff.value++;
                simulationManager.ShowText($"{myStats.name} Gained A Stack of Pyromania From {myStats.eSkill.basic.name}");
            }
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
        if (annieP.Control())
        {
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(Constants.GetAnnieStunDurationByLevel(myStats.level), targetStats.buffManager, myStats.passiveSkill.skillName));
            myStats.buffManager.buffs.Remove("Pyromania");
        }
        else
        {
            if (myStats.buffManager.buffs.TryGetValue("Pyromania", out Buff annieBuff))
            {
                annieBuff.value++;
                simulationManager.ShowText($"{myStats.name} Gained A Stack of Pyromania From {myStats.rSkill.basic.name}");
            }
            else
            {
                myStats.buffManager.buffs.Add("Pyromania", new PyromaniaBuff(myStats.buffManager, myStats.rSkill.basic.name));
            }
        }
        UpdateAbilityTotalDamage(ref qSum, 3, myStats.rSkill, 2);
        myStats.rCD = myStats.rSkill.basic.coolDown[2];
    }
}