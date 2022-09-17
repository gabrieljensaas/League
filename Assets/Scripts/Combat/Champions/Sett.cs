using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sett : ChampionCombat
{
    public float pCD = 0;
    public float knuckleDown = 0;
    public bool leftPunched = false;
    public float grit = 0;
    public List<GritBuff> gritList = new();
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "E", "Q", "A" };

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
        checksR.Add(new CheckIfImmobilize(this));
        checkTakeDamageAAPostMitigation.Add(new CheckForGrit(this, this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckForGrit(this, this));

        qKeys.Add("Bonus Physical Damage");
        wKeys.Add("Damage");
        eKeys.Add("Physical Damage");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        pCD += Time.deltaTime;
        if (pCD >= 2) leftPunched = false;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        pCD = 0;
        leftPunched = false;
        StartCoroutine(KnuckleDown());
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;
        StartCoroutine(HaymakerShield());
        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats, grit), myStats.wSkill[0].basic.name, SkillDamageType.True);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.1f, targetStats.buffManager, myStats.eSkill[0].basic.name));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        yield return StartCoroutine(StartCastingAbility(0.25f));
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        targetStats.buffManager.buffs.Add("Suppression", new SuppressionBuff(1.5f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        yield return StartCoroutine(StartCastingAbility(1.23f));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        yield return StartCoroutine(StartCastingAbility(0.27f));
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        if (targetStats.buffManager.HasImmobilize) yield break;
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
        myStats.buffManager.buffs.Add("Suppression", new SuppressionBuff(1.5f, myStats.buffManager, myStats.rSkill[0].basic.name));
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(1.23f));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(0.27f));
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        if(knuckleDown > 0)
        {
            yield return StartCoroutine(StartCastingAbility(1f));
            AutoAttack((myStats.AD + myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats)) / myStats.AD);
            pCD = 0;
            leftPunched = false;
            knuckleDown--;
        }
        else if (leftPunched)
        {
            yield return StartCoroutine(StartCastingAbility(0.0125f));
            AutoAttack((myStats.AD + (5 * myStats.level) + (myStats.bonusAD)) / myStats.AD);
            pCD = 0;
            leftPunched = false;
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(0.1f));
            AutoAttack();
            leftPunched = true;
            pCD = 0;
        }
    }

    public IEnumerator KnuckleDown()
    {
        knuckleDown += 2;
        yield return new WaitForSeconds(5);
        knuckleDown = 0;
    }

    public IEnumerator HaymakerShield()
    {
        myStats.buffManager.shields.Add(myStats.wSkill[0].basic.name, new ShieldBuff(3, myStats.buffManager, myStats.wSkill[0].basic.name, grit, myStats.wSkill[0].basic.name));
        yield return new WaitForSeconds(0.75f);
        if(myStats.buffManager.shields.TryGetValue(myStats.wSkill[0].basic.name, out ShieldBuff value))
        {
            value.decaying = true;
        }
    }
}