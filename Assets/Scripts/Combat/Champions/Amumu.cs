using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Amumu : ChampionCombat
{
    private int qCharge = 2;
    private float qRecharge = 0;
    private bool wCast = false;

    public static float[] qCD = { 16, 15.5f, 15, 14.5f, 14 };

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
        checksQ.Add(new CheckIfImmobilize(this));

        targetCombat.checkTakeDamageAA.Add(new CheckForAmumuCurse(this));
        targetCombat.checkTakeDamageAbility.Add(new CheckForAmumuCurse(this));

        checkTakeDamageAAPostMitigation.Add(new CheckTantrumPassive(this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckTantrumPassive(this)); //need to implement for ability

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage Per Tick");
        eKeys.Add("Physical Damage Reduction");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        if (qCharge != 2)
        {
            qRecharge += Time.deltaTime;
            if (qRecharge > qCD[myStats.qLevel])
            {
                qCharge++;
                qRecharge = 0;
            }
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        if (qCharge > 0)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            MyBuffManager.Add("Stun", new StunBuff(1f, TargetBuffManager, QSkill().basic.name));

            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], buffNames: new string[] { "Stun" }, skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable);

            qCharge--;
            myStats.qCD = 3f;
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;
        if (wCast) yield break;
        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        wCast = !wCast;
        StartCoroutine(Despair());
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        MyBuffManager.Add(RSkill().basic.name, new KnockdownBuff(0.1f, TargetBuffManager, RSkill().basic.name));
        MyBuffManager.Add("Stun", new StunBuff(1.5f, TargetBuffManager, RSkill().basic.name));
        if(UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable, buffNames: new string[] {"Stun"}) != float.MinValue) ApplyCurse();
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if(AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal, SkillComponentTypes.OnHit | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable)).damage != float.MinValue) ApplyCurse();
    }

    public IEnumerator Despair()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(0), myStats.wLevel, wKeys[0]);
        StartCoroutine(Despair());
    }

    public void DamagedByAutoAttack(float damage)
    {
        if (damage > 0)
        {
            myStats.eCD -= 0.5f;
        }
    }

    public void ApplyCurse()
    {
        TargetBuffManager.Add("AmumuCurse", new AmumuCurseBuff(3f, TargetBuffManager, "Cursed Touch"));
    }
}