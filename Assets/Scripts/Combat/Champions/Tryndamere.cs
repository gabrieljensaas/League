using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tryndamere : ChampionCombat
{
    private float fury = 0;
    private float baseAD;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "W", "A" };

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

        checkTakeDamageAAPostMitigation.Add(new CheckTryndamereUndyingRage(this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckTryndamereUndyingRage(this));

        qKeys.Add("Bonus Attack Damage");
        qKeys.Add("Additional Bonus AD");
        qKeys.Add("Maximum Total Bonus AD");
        qKeys.Add("Minimum Heal");
        qKeys.Add("Heal Per Fury");
        qKeys.Add("Maximum Heal");
        wKeys.Add("Attack Damage Reduction");
        wKeys.Add("Slow");
        eKeys.Add("Physical Damage");
        rKeys.Add("Fury Gained");
        rKeys.Add("Minimum Health Threshold");

        base.UpdatePriorityAndChecks();
        myStats.AD += 30; //hard coded for now
        baseAD = myStats.AD;
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        myStats.AD = baseAD + (int)myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats); //bloodlust AD
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (AutoAttack().isCrit)
        {
            myStats.eCD -= 1.5f;
            AddFury(10);
        }
        else AddFury(5);
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateTotalHeal(ref hSum, myStats.qSkill[0].UseSkill(4, qKeys[3], myStats, targetStats) + (fury * (myStats.qSkill[0].UseSkill(4, qKeys[4], myStats, targetStats) + (0.012f * myStats.AP))) , myStats.qSkill[0].basic.name);
        fury = 0;
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("MockingShout", new AttackDamageBuff(4, targetStats.buffManager, myStats.wSkill[0].name, -(int)myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats), "MockingShout"));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        yield return base.ExecuteE();
        AddFury(2);
    }

    public override IEnumerator ExecuteR()
    {
        if(myStats.PercentCurrentHealth < 0.15f) //if tryndamere is at 15% health
        {
            if (!CheckForAbilityControl(checksR)) yield break;

            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
            myStats.buffManager.buffs.Add("UndyingRage", new UndyingRageBuff(5, myStats.buffManager, myStats.rSkill[0].name));
            AddFury(myStats.rSkill[0].UseSkill(4, rKeys[0], myStats, targetStats));
            myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        }
    }

    private void AddFury(float furyAdd)
    {
        fury += furyAdd;
        if (fury > 100) fury = 100;
    }
}
