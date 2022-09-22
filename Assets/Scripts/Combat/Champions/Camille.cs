using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Camille : ChampionCombat
{
    private bool qCast;
    private float timeSinceQ;
    private float timeSinceE;
    private bool eCast;
    private float timeInsideR;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "R", "E", "W", "A" };

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
        checksA.Add(new CheckIfDisarmed(this));
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Bonus Physical Damage");
        qKeys.Add("Increased Mixed Damage");
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
        timeSinceE += Time.deltaTime;
        timeSinceQ += Time.deltaTime;
        timeInsideR -= Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        if (!qCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime)); //need to implement uncancellable windup
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
            StartCoroutine(PrecisionProtocol());
            myStats.qCD = 0.25f;
            timeSinceQ = 0;
            attackCooldown = 0;
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            float multiplier =0.36f+(0.04f *myStats.level);
            UpdateAbilityTotalDamage(ref rSum, 0, myStats.qSkill[0].UseSkill(2, qKeys[1], myStats, targetStats) *  (multiplier > 1 ? 1 : multiplier), myStats.qSkill[0].basic.name, SkillDamageType.True);
            StopCoroutine(PrecisionProtocol());
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4] - timeSinceQ;
            attackCooldown = 0;
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]); //Ignored OuterCore Bonus damage (Considering champion doesn't move)
        myStats.buffManager.buffs.Add("CantAA", new CantAABuff(1.1f, myStats.buffManager, "CantAA"));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        if (!eCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            StartCoroutine(HookShot());
            myStats.eCD = 0.4f;
            timeSinceE = 0;
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
            myStats.buffManager.buffs.Add("StunBuff", new StunBuff(0.75f, myStats.buffManager, myStats.eSkill[0].basic.name));
            myStats.buffManager.buffs.Add("AttackSpeedBuff", new AttackSpeedBuff(5f, myStats.buffManager, myStats.eSkill[0].basic.name, myStats.eSkill[0].UseSkill(4, qKeys[1], myStats, targetStats), "AttackSpeedBuff"));
            StopCoroutine(HookShot());
            myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(0.5f, myStats.buffManager, "Untargetable"));
        timeInsideR = myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
        if (timeInsideR <= 0)
        {
            float damage = myStats.rSkill[0].UseSkill(2, rKeys[1], myStats, targetStats);
            UpdateAbilityTotalDamage(ref rSum, 3, damage, myStats.rSkill[0].basic.name, SkillDamageType.Spell);
        }
    }

    public IEnumerator PrecisionProtocol()
    {
        qCast = true;
        yield return new WaitForSeconds(3.5f);
        qCast = false;
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4] - timeSinceQ;
    }

    public IEnumerator HookShot()
    {
        eCast = true;
        yield return new WaitForSeconds(0.75f);
        eCast = false;
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
    }
}