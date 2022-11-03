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

        targetCombat.checkTakeDamage.Add(new CheckForAmumuCurse(targetCombat));

        checkTakeDamagePostMitigation.Add(new CheckTantrumPassive(this)); //need to implement for ability

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
            qRecharge += Time.fixedDeltaTime;
            if (qRecharge > qCD[myStats.qLevel])
            {
                qCharge++;
                qRecharge = 0;
            }
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        if (qCharge > 0)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            TargetBuffManager.Add("Stun", new StunBuff(1f, TargetBuffManager, QSkill().basic.name));

            UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], buffNames: new string[] { "Stun" }, skillComponentTypes: (SkillComponentTypes)34950);

            qCharge--;
            myStats.qCD = 3f;
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;
        if (wCast) yield break;
        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        wCast = !wCast;
        StartCoroutine(Despair());
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[1], skillComponentTypes: (SkillComponentTypes)18560);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        TargetBuffManager.Add("Stun", new StunBuff(1.5f, TargetBuffManager, RSkill().basic.name));
        if (UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)18560, buffNames: new string[] { "Stun" }) != float.MinValue)
            ApplyCurse();
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Amumu's Auto Attack") != float.MinValue)
            ApplyCurse();
    }

    public IEnumerator Despair()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateTotalDamage(ref wSum, 1, WSkill(0), myStats.wLevel, wKeys[0],skillComponentTypes: (SkillComponentTypes)8192);
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