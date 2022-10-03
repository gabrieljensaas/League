using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Amumu : ChampionCombat
{
    private int qCharge = 2;
    private bool wCast = false;
    private float timeSinceCurse;
    private bool hasCurse = false;
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

        checkTakeDamageAAPostMitigation.Add(new CheckTantrumPassive(this, this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckTantrumPassive(this, this)); //need to implement for ability

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage Per Tick");
        eKeys.Add("Physical Damage Reduction");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");

        if (timeSinceCurse > 3 && targetStats.buffManager.buffs.TryGetValue("CurseBuff", out Buff curseBuff))

        {
            curseBuff.Kill();
        }

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        timeSinceCurse += Time.deltaTime;
        qCharge++;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        if (qCharge > 0)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            MyBuffManager.Add("StunBuff", new StunBuff(1f, TargetBuffManager, QSkill().basic.name));
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
            if (hasCurse)
            {
                UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats) * 0.1f, SkillDamageType.True), QSkill().basic.name);
            }
            myStats.qCD = QSkill().basic.coolDown[4];
            qCharge--;
            yield return new WaitForSeconds(3f);

        }
    }


    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;
        if (wCast) yield break;
        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        wCast = !wCast;
        StartCoroutine(Despair());
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
        if (hasCurse)
        {
            UpdateAbilityTotalDamage(ref eSum, 2, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats) * 0.1f, SkillDamageType.True), ESkill().basic.name);
        }
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        MyBuffManager.Add(RSkill().basic.name, new KnockdownBuff(0.1f, TargetBuffManager, RSkill().basic.name));
        MyBuffManager.Add("StunBuff", new StunBuff(1.5f, TargetBuffManager, "StunBuff"));
        ApplyCurse();
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        ApplyCurse();
    }

    public IEnumerator Despair()
    {
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(0), myStats.wLevel, wKeys[0]);
        if (hasCurse)
        {
            UpdateAbilityTotalDamage(ref wSum, 1, new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * 0.1f, SkillDamageType.True), WSkill().basic.name);
        }
        timeSinceCurse = 0;
        StartCoroutine(Despair());
    }

    public void DamagedByAutoAttack(float damage)
    {
        if (damage > 0)
        {
            myStats.eCD -= 0.5f;
        }
    }

    public void DamagedByAbility(float damage)
    {
        if (damage > 0)
        {
            //waiting for gabriel to give instruction
        }
    }

    public void ApplyCurse()
    {
        if (!hasCurse)
        {
            MyBuffManager.Add("CurseBuff", new AmumuCurseBuff(3f, TargetBuffManager, "CurseBuff"));
            timeSinceCurse = 0;
        }
    }

}