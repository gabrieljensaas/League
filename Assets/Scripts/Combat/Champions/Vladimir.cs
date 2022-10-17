using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Vladimir : ChampionCombat
{
    private float bloodthirstCD = 0;
    private float bloodthirstStack = 0;
    private bool isStasis = false;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "E", "W", "A" };

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
        checksQ.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));
        targetCombat.checkTakeDamageAA.Add(new CheckIfHemoplague(targetCombat));
        targetCombat.checkTakeDamageAbility.Add(new CheckIfHemoplague(targetCombat));

        float extraAP = myStats.bonusHP * 0.033f;           //passive
        myStats.maxHealth += myStats.AP * 1.6f;
        myStats.currentHealth = myStats.maxHealth;
        myStats.AP += extraAP;

        qKeys.Add("Magic Damage");
        qKeys.Add("Heal");
        qKeys.Add("Empowered Damage");
        wKeys.Add("Magic Damage Per Tick");
        eKeys.Add("Maximum Magic Damage");
        rKeys.Add("Magic damage");
        rKeys.Add("Heal");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        bloodthirstCD -= Time.fixedDeltaTime * (isStasis || myStats.buffManager.buffs.ContainsKey("Stasis") ? 0.25f : 1);
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if (bloodthirstCD > 0)
        {
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[2]);
            UpdateTotalHeal(ref hSum, myStats.qSkill[0].UseSkill(4, qKeys[1], myStats, targetStats) + 20 + (10 * myStats.level) + ((myStats.maxHealth - myStats.currentHealth) * (5 + (4 * (myStats.AP % 100))) * 0.01f), myStats.qSkill[0].basic.name);
            bloodthirstCD = 0;
            bloodthirstStack = 0;
        }
        else
        {
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
            UpdateTotalHeal(ref hSum, myStats.qSkill[0], 4, qKeys[1]);
        }
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        StartCoroutine(AddBloodthirstStack(myStats.qSkill[0].basic.coolDown[4]));
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(2, myStats.buffManager, myStats.wSkill[0].basic.name));
        isStasis = true;
        myStats.currentHealth *= 0.8f;
        StartCoroutine(SanguinePool());
        myStats.buffManager.buffs.Add("UnableToAct", new UnableToActBuff(2, myStats.buffManager, myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        if (myStats.currentHealth > myStats.maxHealth * 0.12f) myStats.currentHealth *= 0.92f;
        isStasis = true;
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(1, myStats.buffManager, myStats.eSkill[0].basic.name, "TidesOfBlood"));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Hemoplague", new HemoplagueBuff(4, targetStats.buffManager, myStats.rSkill[0].basic.name));
        StartCoroutine(Hemoplague());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Hemoplague", new HemoplagueBuff(4, myStats.buffManager, myStats.rSkill[0].basic.name));
        StartCoroutine(HHemoplague(skillLevel));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
    }

    public IEnumerator AddBloodthirstStack(float cd)
    {
        yield return new WaitForSeconds(cd);
        bloodthirstStack++;
        if (bloodthirstStack == 2)
        {
            bloodthirstCD = 2.5f;
        }
    }

    public IEnumerator SanguinePool()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats) * 0.15f, myStats.wSkill[0].basic.name);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats) * 0.15f, myStats.wSkill[0].basic.name);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats) * 0.15f, myStats.wSkill[0].basic.name);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats) * 0.15f, myStats.wSkill[0].basic.name);
        isStasis = false;
    }

    public IEnumerator TidesOfBlood()
    {
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        isStasis = false;
    }

    public IEnumerator Hemoplague()
    {
        yield return new WaitForSeconds(4f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        yield return new WaitForSeconds(0.4f);
        UpdateTotalHeal(ref hSum, myStats.rSkill[0], 2, rKeys[1]);
    }

    public IEnumerator HHemoplague(int skillLevel)
    {
        yield return new WaitForSeconds(4f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        yield return new WaitForSeconds(0.4f);
        UpdateTotalHealSylas(ref targetCombat.hSum, myStats.rSkill[0], skillLevel, rKeys[1]);
    }
    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }
}