using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Vi : ChampionCombat
{
    public int wStack = 0;
    public float passiveCD = 0;
    public int eStack = 2;
    public float eRecharge = 0;
    public bool eCast = false;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "E", "A", "" };

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
        checksQ.Add(new CheckIfImmobilize(this));
        checksR.Add(new CheckIfImmobilize(this));
        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));
        checksQ.Add(new CheckIfUnableToAct(this));
        checksW.Add(new CheckIfUnableToAct(this));
        checksE.Add(new CheckIfUnableToAct(this));
        checksR.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));

        qKeys.Add("Maximum Physical Damage");
        wKeys.Add("Bonus Physical Damage");
        wKeys.Add("Bonus Attack Speed");
        eKeys.Add("Physical Damage");
        rKeys.Add("");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        passiveCD -= Time.fixedDeltaTime;
        eRecharge -= Time.fixedDeltaTime;
        if (eRecharge <= 0 && eStack < 2)
        {
            eStack++;
            eRecharge = myStats.eSkill[0].basic.coolDown[4];
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(3, myStats.buffManager, myStats.qSkill[0].basic.name, "VaultBreaker"));
        StartCoroutine(VaultBreaker());
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;
        if (eStack < 1 && !eCast) yield break;
        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        StartCoroutine(RelentlessForce());
        myStats.eCD = 6;
        attackCooldown = 0;
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;        //needs research of exact timings

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("UnableToAct", new UnableToActBuff(0.5f, myStats.buffManager, myStats.rSkill[0].basic.name));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        yield return new WaitForSeconds(0.3f);
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(1.3f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        yield return new WaitForSeconds(0.75f);
        if (passiveCD <= 0) BlastShield();
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (eCast)
        {
            if (passiveCD <= 0) BlastShield();
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
            eCast = false;
            StopCoroutine(RelentlessForce());
            myStats.eCD = 1;
        }
        else AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        StopCoroutine(DentingBlows());
        StartCoroutine(DentingBlows());
    }

    public void BlastShield()
    {
        myStats.buffManager.shields.Add("BlastShield", new ShieldBuff(3, myStats.buffManager, "Blast Shield", myStats.maxHealth * 0.13f, "BlastShield"));
        passiveCD = 16.5f - (0.5f * myStats.level > 9f ? 9f : myStats.level);
    }
    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }

    public IEnumerator VaultBreaker()
    {
        yield return new WaitForSeconds(1.25f);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.75f, targetStats.buffManager, myStats.qSkill[0].basic.name));
        if (passiveCD <= 0) BlastShield();
        StopCoroutine(DentingBlows());
        StartCoroutine(DentingBlows());
    }

    public IEnumerator DentingBlows()
    {
        wStack++;
        if (wStack == 3)
        {
            UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
            targetStats.buffManager.buffs.Add(myStats.wSkill[0].basic.name, new ArmorReductionBuff(4, targetStats.buffManager, myStats.wSkill[0].basic.name, 20, myStats.wSkill[0].basic.name));
            myStats.buffManager.buffs.Add(myStats.wSkill[0].basic.name, new AttackSpeedBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[1], myStats, targetStats) * myStats.baseAttackSpeed, myStats.wSkill[0].basic.name));
            wStack = 0;
            passiveCD -= 3;
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(4f);
            wStack = 0;
        }
    }

    public IEnumerator RelentlessForce()
    {
        eCast = true;
        yield return new WaitForSeconds(6f);
        eCast = false;
        myStats.eCD = 1f;
    }
}