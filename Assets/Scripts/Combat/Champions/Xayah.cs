using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Xayah : ChampionCombat
{
    public static float GetXayahPassiveADPercent(int level)
    {
        return level switch
        {
            < 7 => 30,
            < 13 => 40,
            _ => 50
        };
    }

    private int feathersInGround = 0;
    public int feathersAtHand = 0;
    private bool pulledFeathers = false;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "R", "W", "Q", "A" };

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
        autoattackcheck = new XayahAACheck(this, this);
        checksQ.Add(new CheckIfUnableToAct(this));
        checksW.Add(new CheckIfUnableToAct(this));
        checksE.Add(new CheckIfUnableToAct(this));
        checksR.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Maximum Total Damage");
        wKeys.Add("Bonus Attack Speed");
        eKeys.Add("Physical Damage Per Feather");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        StartCoroutine(FeatherInGround());
        StartCoroutine(FeatherInGround());
        StopCoroutine(FeatherAtHand());
        StartCoroutine(FeatherAtHand());
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add(myStats.wSkill[0].basic.name, new AttackSpeedBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats) * (feathersInGround - (FindMultiplier(feathersInGround - 1) * 0.05f)), myStats.eSkill[0].basic.name, SkillDamageType.Phyiscal);
        StartCoroutine(PulledFeathers());
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(1.5f, myStats.buffManager, myStats.rSkill[0].basic.name));
        myStats.buffManager.buffs.Add("UnableToActBuff", new UnableToActBuff(1.5f, myStats.buffManager, myStats.rSkill[0].basic.name));
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        StartCoroutine(FeatherInGround());
        StartCoroutine(FeatherInGround());
        StartCoroutine(FeatherInGround());
        StartCoroutine(FeatherInGround());
        StartCoroutine(FeatherInGround());
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
    }

    public IEnumerator FeatherInGround()
    {
        feathersInGround++;
        yield return new WaitForSeconds(6f);
        if (!pulledFeathers) feathersInGround--;
    }
    public IEnumerator FeatherAtHand()
    {
        feathersAtHand = feathersAtHand > 2 ? 5 : feathersAtHand + 3;
        yield return new WaitForSeconds(8f);
        feathersAtHand = 0;
    }

    public int FindMultiplier(int featherCount)
    {
        int multiplier = 0;
        while (featherCount > 0)
        {
            multiplier += featherCount;
            featherCount--;
        }
        return multiplier;
    }

    public IEnumerator PulledFeathers()
    {
        pulledFeathers = true;
        yield return new WaitForSeconds(6f);
        pulledFeathers = false;
    }
}