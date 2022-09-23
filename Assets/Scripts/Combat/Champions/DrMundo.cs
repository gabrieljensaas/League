using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class DrMundo : ChampionCombat
{
    private float pCD = 0;
    public float GreyHealth = 0;
    public bool WActive = false;
    private float bonusAD = 0;
    public bool EActive = false;
    public static float GetPassiveCDByLevel(int level)
    {
        return level switch
        {
            < 3 => 60,
            < 6 => 52.5f,
            < 8 => 45f,
            < 11 => 37.5f,
            < 13 => 30f,
            < 16 => 22.5f,
            _ => 15f
        };
    }

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
        checkTakeDamageAAPostMitigation.Add(new CheckForMundosGreyHealth(this, this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckForMundosGreyHealth(this, this));
        autoattackcheck = new DrMundoAACheck(this, this);

        qKeys.Add("Magic Damage");
        qKeys.Add("Minimum Damage");
        wKeys.Add("Magic Damage per Tick");
        wKeys.Add("Damage Stored");
        wKeys.Add("Magic Damage");
        eKeys.Add("Bonus Attack Damage");
        eKeys.Add("Maximum Additional Bonus AD");
        eKeys.Add("Minimum Bonus Physical Damage");
        rKeys.Add("Increased Base Health");
        rKeys.Add("Bonus Attack Damage");
        rKeys.Add("Regeneration per Second");

        myStats.AD += myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats);

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        myStats.AD -= bonusAD;
        bonusAD = myStats.eSkill[0].UseSkill(4, eKeys[1], myStats, targetStats,cap: 70);
        myStats.AD += bonusAD;
        if (pCD <= 0) GoesWhereHePleases();
        base.CombatUpdate();
        pCD -= Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (pCD <= 0) GoesWhereHePleases();
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.currentHealth -= 50;
        CheckDeath();
        var a = myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats);
        var b = myStats.qSkill[0].UseSkill(4, qKeys[1], myStats, targetStats);
        UpdateAbilityTotalDamage(ref qSum, 0, a > b ? a : b, myStats.qSkill[0].basic.name, SkillDamageType.Spell);
        UpdateTotalHeal(ref hSum, 50, myStats.qSkill[0].basic.name);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (pCD <= 0) GoesWhereHePleases();
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.currentHealth *= 0.95f;
        GreyHealth = 0;
        WActive = true;
        StartCoroutine(HearthZapper(0));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (pCD <= 0) GoesWhereHePleases();
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.currentHealth -= (4 + 1) * 10;
        CheckDeath();
        StartCoroutine(BluntForceTrauma());
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        attackCooldown = 0;
    }

    public override IEnumerator ExecuteR()
    {
        if (pCD <= 0) GoesWhereHePleases();
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        StartCoroutine(MaximumDosage());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (pCD <= 0) GoesWhereHePleases();
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
    }

    public void GoesWhereHePleases()
    {
        foreach (var item in myStats.buffManager.buffs.Keys)
        {
            if (myStats.buffManager.buffs[item] is AirborneBuff
                                                or BerserkBuff
                                                or CharmBuff
                                                or FleeBuff
                                                or KnockdownBuff
                                                or RootBuff
                                                or SleepBuff
                                                or StasisBuff
                                                or StunBuff
                                                or SuppressionBuff
                                                or SuspensionBuff
                                                or TauntBuff)
            {
                myStats.buffManager.buffs.Remove(item);
                myStats.currentHealth *= 0.93f;
                pCD = GetPassiveCDByLevel(myStats.level);
                StartCoroutine(TakeCanister());
                break;
            }
        }
    }

    public IEnumerator TakeCanister()
    {
        yield return new WaitForSeconds(1f);
        UpdateTotalHeal(ref hSum, myStats.currentHealth * 0.08f, "Mundo Goes Where He Pleases");
        pCD -= 15;
    }

    public IEnumerator HearthZapper(float time)
    {
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        if (time != 4) StartCoroutine(HearthZapper(time + 0.25f));
        else
        {
            WActive = false;
            UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[2]);
            UpdateTotalHeal(ref hSum, GreyHealth, myStats.wSkill[0].basic.name);
        }
    }

    public IEnumerator BluntForceTrauma()
    {
        EActive = true;
        yield return new WaitForSeconds(4f);
        EActive = false;
    }

    public IEnumerator MaximumDosage()
    {
        myStats.baseHealth += myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
        myStats.maxHealth += myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
        myStats.AD += myStats.rSkill[0].UseSkill(2, rKeys[1], myStats, targetStats);
        yield return new WaitForSeconds(1);
        UpdateTotalHeal(ref hSum, myStats.rSkill[0], 2, rKeys[2]);
        yield return new WaitForSeconds(1);
        UpdateTotalHeal(ref hSum, myStats.rSkill[0], 2, rKeys[2]);
        yield return new WaitForSeconds(1);
        UpdateTotalHeal(ref hSum, myStats.rSkill[0], 2, rKeys[2]);
        yield return new WaitForSeconds(1);
        UpdateTotalHeal(ref hSum, myStats.rSkill[0], 2, rKeys[2]);
        yield return new WaitForSeconds(1);
        UpdateTotalHeal(ref hSum, myStats.rSkill[0], 2, rKeys[2]);
        yield return new WaitForSeconds(1);
        UpdateTotalHeal(ref hSum, myStats.rSkill[0], 2, rKeys[2]);
        yield return new WaitForSeconds(1);
        UpdateTotalHeal(ref hSum, myStats.rSkill[0], 2, rKeys[2]);
        yield return new WaitForSeconds(1);
        UpdateTotalHeal(ref hSum, myStats.rSkill[0], 2, rKeys[2]);
        yield return new WaitForSeconds(1);
        UpdateTotalHeal(ref hSum, myStats.rSkill[0], 2, rKeys[2]);
        yield return new WaitForSeconds(1);
        UpdateTotalHeal(ref hSum, myStats.rSkill[0], 2, rKeys[2]);
        myStats.baseHealth -= myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
        myStats.maxHealth -= myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
        myStats.AD -= myStats.rSkill[0].UseSkill(2, rKeys[1], myStats, targetStats);
    }
}