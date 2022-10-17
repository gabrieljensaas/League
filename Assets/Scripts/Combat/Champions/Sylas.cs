using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Sylas : ChampionCombat
{
    public int UnshackledStack = 0;
    public bool eCast = false;
    private float timeSinceE = 0f;
    CheckIfEmpoweredTumble empoweredTumbleCheck;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

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
        checksW.Add(new CheckIfImmobilize(this));
        checksE.Add(new CheckIfImmobilize(this));
        autoattackcheck = new SylasAACheck(this, this);
        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));
        checkTakeDamageAbility.Add(new CheckShield(this));
        checkTakeDamageAA.Add(new CheckShield(this));
        checksQ.Add(new CheckIfRagnorok(this));
        checksW.Add(new CheckIfRagnorok(this));
        checksE.Add(new CheckIfRagnorok(this));
        checksR.Add(new CheckIfRagnorok(this));
        checksA.Add(new CheckIfRagnorok(this));
        empoweredTumbleCheck = new CheckIfEmpoweredTumble(this);
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checksQ.Add(new CheckIfUnableToAct(this));
        checksW.Add(new CheckIfUnableToAct(this));
        checksE.Add(new CheckIfUnableToAct(this));
        checksR.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage");
        wKeys.Add("Minimum Heal");
        eKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();

        timeSinceE += Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if (empoweredTumbleCheck.Control()) myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(1, myStats.buffManager, myStats.qSkill[0].basic.name));
        Passive();
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        CheckBlightStacks();
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(0.6F);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[1]);
        CheckBlightStacks();
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        Passive();
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        CheckBlightStacks();
        UpdateTotalHeal(ref hSum, (1 + Mathf.Clamp01(((myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth) % 0.006f)) * myStats.wSkill[0].UseSkill(4, wKeys[1], myStats, targetStats), myStats.wSkill[0].basic.name);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        if (!eCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            StartCoroutine(Abscond());
            myStats.eCD = 0.2f;
            timeSinceE = 0;
            if (targetStats.buffManager.buffs.TryGetValue("Plasma", out Buff value)) value.duration = 4;
            else targetStats.buffManager.buffs.Add("Plasma", new PlasmaBuff(4, targetStats.buffManager, myStats.eSkill[1].basic.name));
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[1].basic.castTime));
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
            CheckBlightStacks();
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.5f, targetStats.buffManager, myStats.eSkill[1].basic.name));
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.5f, targetStats.buffManager, myStats.eSkill[1].basic.name));
            eCast = false;
            StopCoroutine(Abscond());
            myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetCombat.HijackedR(2);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield break;
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        CheckVitals();
        if (targetStats.buffManager.buffs.TryGetValue("Plasma", out Buff value)) value.duration = 4;
        else targetStats.buffManager.buffs.Add("Plasma", new PlasmaBuff(4, targetStats.buffManager, "Sylas' Auto Attack"));
        if (myStats.buffManager.buffs.TryGetValue("Ragnorok", out Buff valu))
        {
            valu.duration += 2.5f;
        }
        if (myStats.buffManager.buffs.TryGetValue("Ragnorok ", out Buff val))
        {
            val.duration += 2.5f;
        }
    }

    public void Passive()
    {
        if (UnshackledStack != 3) UnshackledStack++;
        StopCoroutine(Unshackled());
        StartCoroutine(Unshackled());
    }

    public IEnumerator Unshackled()
    {
        myStats.buffManager.buffs.TryAdd("Unshackled", new AttackSpeedBuff(float.MaxValue, myStats.buffManager, "Petricite Burst", myStats.baseAttackSpeed * 1.25f, "Unshackled"));
        yield return new WaitForSeconds(4);
        UnshackledStack = 0;
    }

    public IEnumerator Abscond()
    {
        eCast = true;
        yield return new WaitForSeconds(3.5f);
        eCast = false;
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
    }

    private void CheckVitals()
    {
        if (targetStats.buffManager.buffs.TryGetValue("VitalsGrandChallenge", out Buff vitalsUlt))
        {
            VitalsBuff vitalsBuff = (VitalsBuff)vitalsUlt;
            if (vitalsBuff.isActive)
            {
                vitalsBuff.value--;

                if (vitalsBuff.value <= 0)
                {
                    vitalsBuff.Kill();
                    StartCoroutine(GrandChallengeHealing((int)(5 - vitalsBuff.value)));
                }
            }
        }
    }

    private IEnumerator GrandChallengeHealing(int vitalsHit)
    {
        for (int i = 0; i < vitalsHit; i++)
        {
            yield return new WaitForSeconds(1);
            UpdateTotalHeal(ref hSum, targetStats.rSkill[0].SylasUseSkill(4, rKeys[0], myStats, targetStats), "Victory Zone"); //hardcoded healing
        }
    }

    private void CheckBlightStacks(float multiplier = 1)
    {
        if (targetStats.buffManager.buffs.TryGetValue("Blight", out Buff value))
        {
            UpdateAbilityTotalDamage(ref wSum, 1, targetStats.wSkill[0], 4, targetCombat.wKeys[1], multiplier * value.value);           //ability level is sylas's ability level
            myStats.qCD -= value.value * myStats.qSkill[0].basic.coolDown[4] * 0.12f;
            myStats.wCD -= value.value * myStats.wSkill[0].basic.coolDown[4] * 0.12f;
            myStats.eCD -= value.value * myStats.eSkill[0].basic.coolDown[4] * 0.12f;
            value.Kill();
        }
    }

    public override void StopChanneling(string uniqueKey)
    {
        if ("Comeuppance" == uniqueKey) myStats.rCD = 5f;
        targetCombat.StopCoroutine(uniqueKey);
    }
}