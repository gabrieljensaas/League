using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Camille : ChampionCombat
{
    public static float[] QTrueDamageMultiplier = { 0.4f, 0.44f, 0.48f, 0.52f, 0.56f, 0.6f, 0.64f, 0.68f, 0.72f, 0.76f, 0.8f, 0.84f, 0.88f, 0.92f, 0.96f, 1, 1, 1 };
    private float pCD = 0;
    private int qCast = 0;
    private float timeSinceQ;
    private float timeInsideR;
    private bool waitingForEmpoweredQ = false;
    public static float GetCamillePassiveCooldown(int level)
    {
        return level switch
        {
            < 7 => 20f,
            < 13 => 15f,
            _ => 10f
        };
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "Q", "A" };

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
        checksE.Add(new CheckIfImmobilize(this));
        checksR.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checksA.Add(new CheckIfCantAA(this));
        checksQ.Add(new CheckIfUnableToAct(this));
        checksW.Add(new CheckIfUnableToAct(this));
        checksE.Add(new CheckIfUnableToAct(this));
        checksR.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Bonus Physical Damage");
        wKeys.Add("Physical Damage");
        eKeys.Add("Physical Damage");
        eKeys.Add("Bonus Attack Speed");
        rKeys.Add("Zone Duration");
        rKeys.Add("Bonus Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceQ += Time.deltaTime;
        timeInsideR -= Time.deltaTime;
        pCD -= Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;
        if (waitingForEmpoweredQ) yield break;
        if (qCast == 0)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
            qCast = 1;
            StartCoroutine(PrecisionProtocol());
            timeSinceQ = 0;
            attackCooldown = 0;
            myStats.qCD = 3.9999999f;
        }
        else if (qCast == 1)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
            StartCoroutine(PrecisionProtocol());
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4] - timeSinceQ;
            attackCooldown = 0;
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
            StartCoroutine(PrecisionProtocol());
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4] - timeSinceQ;
            attackCooldown = 0;
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("CantAA", new CantAABuff(1.1f, myStats.buffManager, "CantAA"));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(1.1f);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]); //Ignored OuterCore Bonus damage (Considering champion doesn't move)
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[1], 4, eKeys[0]);
        targetStats.buffManager.buffs.Add("StunBuff", new StunBuff(0.75f, targetStats.buffManager, myStats.eSkill[1].basic.name));
        myStats.buffManager.buffs.Add(myStats.eSkill[1].basic.name, new AttackSpeedBuff(5f, myStats.buffManager, myStats.eSkill[1].basic.name, myStats.eSkill[1].UseSkill(4, qKeys[1], myStats, targetStats), myStats.eSkill[1].basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Disrupt", new DisruptBuff(0.1f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(0.5f, myStats.buffManager, myStats.rSkill[0].basic.name));
        myStats.buffManager.buffs.Add("UnableToAct", new UnableToActBuff(0.5f, myStats.buffManager, myStats.rSkill[0].basic.name));
        timeInsideR = myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (qCast == 0) AutoAttack();
        else if (qCast == 1)
        {
            StopCoroutine(PrecisionProtocol());
            AutoAttack(1 + myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats));
            if (timeSinceQ < 2.5f) StartCoroutine(WaitForEmpoweredQ());
            qCast = 2;
            myStats.qCD = 0.25f;
        }
        else if (qCast == 2)
        {
            StopCoroutine(PrecisionProtocol());
            AutoAttack(1 + myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats));
            qCast = 0;
        }
        else if (qCast == 3)
        {
            StopCoroutine(PrecisionProtocol());
            UpdateAbilityTotalDamage(ref qSum, 0, (myStats.AD + (2 * myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats))) * QTrueDamageMultiplier[myStats.level], myStats.qSkill[0].basic.name, SkillDamageType.True);
            AutoAttack((1 - QTrueDamageMultiplier[myStats.level]) * (1 + (myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats) * 2)));
            qCast = 0;
        }

        GetPassiveShield();
        if (timeInsideR > 0)
        {
            float damage = myStats.rSkill[0].UseSkill(2, rKeys[1], myStats, targetStats);
            UpdateAbilityTotalDamage(ref rSum, 3, damage, myStats.rSkill[0].basic.name, SkillDamageType.Spell);
        }
    }

    public IEnumerator PrecisionProtocol()
    {
        yield return new WaitForSeconds(4f);
        qCast = 0;
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4] - timeSinceQ;
    }

    public IEnumerator WaitForEmpoweredQ()
    {
        waitingForEmpoweredQ = true;
        yield return new WaitForSeconds(1.5f);
        waitingForEmpoweredQ = false;
        qCast = 3;
    }

    public void GetPassiveShield()
    {
        if (pCD <= 0)
        {
            myStats.buffManager.shields.Add("Adaptive Defenses", new ShieldBuff(2f, myStats.buffManager, "Adaptive Defenses", myStats.maxHealth * 0.2f, "Adaptive Defenses")); //Yet to check if target is physical or AP and give shield accordingly
            pCD = GetCamillePassiveCooldown(myStats.level);
        }
    }
}