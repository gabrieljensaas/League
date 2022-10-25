using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Galio : ChampionCombat
{
    private static float DamageWithWChannelTime(float time)
    {
        return time switch
        {
            < 0.25f => 0,
            < 0.5f => 0.25f,
            < 0.75f => 0.5f,
            < 1f => 0.75f,
            < 1.25f => 1f,
            < 1.5f => 1.25f,
            < 1.75f => 1.5f,
            < 2f => 1.75f,
            _ => 2f,
        };
    }

    private static float TauntWithWChannelTime(float time)
    {
        return time switch
        {
            < 0.25f => 0.5f,
            < 0.5f => 0.63f,
            < 0.75f => 0.75f,
            < 1f => 0.88f,
            < 1.25f => 1f,
            < 1.5f => 1.13f,
            < 1.75f => 1.25f,
            < 2f => 1.38f,
            _ => 1.5f,
        };
    }
    private bool hasEmpoweredAuto = false;
    private bool wCast;
    private float timeSinceChannel;
    public bool hasShieldOfDurandPassive = true;
    private float passiveCooldown;

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
        checksR.Add(new CheckIfImmobilize(this));
        checkTakeDamage.Add(new CheckForKassadinPassive(this));
        checkTakeDamage.Add(new CheckForKassadinPassive(this));
        checkTakeDamagePostMitigation.Add(new CheckShield(this));
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Total Magic Damage");
        wKeys.Add("Magic Shield Strength");
        wKeys.Add("Magic Damage Reduction");
        wKeys.Add("Minimum Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");

        if (hasShieldOfDurandPassive)
        {
            MyBuffManager.Add(WSkill().basic.name, new ShieldBuff(float.MaxValue, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), WSkill().basic.name, shieldType: ShieldBuff.ShieldType.Magic));
        }

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        passiveCooldown -= Time.fixedDeltaTime;
        timeSinceChannel += Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[0], myStats, targetStats), SkillDamageType.Spell), QSkill().basic.name);
        yield return new WaitForSeconds(2f);
        UpdateTotalDamage(ref qSum, 0, new Damage(0.1f * targetStats.maxHealth, SkillDamageType.Spell), QSkill().basic.name);
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;
        if (!wCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
            MyBuffManager.Add("Channeling", new ChannelingBuff(3f, MyBuffManager, WSkill().basic.name, "Channeling"));
            MyBuffManager.Add("MRBuff", new MagicResistanceBuff(3f, MyBuffManager, WSkill().basic.name, (int)WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), "MRBuff"));
            MyBuffManager.Add("ArmorBuff", new ArmorBuff(3f, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats) * 0.5f, "ArmorBuff"));
            timeSinceChannel = 0;

        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
            UpdateTotalDamage(ref wSum, 1, new Damage(DamageWithWChannelTime(timeSinceChannel) * WSkill().UseSkill(myStats.wLevel, wKeys[2], myStats, targetStats), SkillDamageType.Spell), WSkill().basic.name);
            TargetBuffManager.Add("TauntBuff", new TauntBuff(TauntWithWChannelTime(timeSinceChannel), TargetBuffManager, WSkill().basic.name));
            myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], skillComponentTypes: SkillComponentTypes.Dash);
        TargetBuffManager.Add(ESkill().basic.name, new AirborneBuff(0.75f, TargetBuffManager, ESkill().basic.name));
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        MyBuffManager.Add(RSkill().basic.name, new ChannelingBuff(1.75f, MyBuffManager, RSkill().basic.name, RSkill().basic.name));
        yield return new WaitForSeconds(1.75f);
        MyBuffManager.Add(RSkill().basic.name, new UntargetableBuff(1f, MyBuffManager, RSkill().basic.name));
        yield return new WaitForSeconds(1);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Dash);
        TargetBuffManager.Add(RSkill().basic.name, new AirborneBuff(0.75f, TargetBuffManager, RSkill().basic.name));
        hasShieldOfDurandPassive = true;
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        CheckColossalSmash();
        if (hasEmpoweredAuto)
        {
            AutoAttack(new Damage(15 + 10.88235f * (myStats.level - 1), SkillDamageType.Spell)); //need to apply crit
            hasEmpoweredAuto = false;
        }
        else
            AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
    }
    private void CheckColossalSmash()
    {
        if (passiveCooldown <= 0)
        {
            hasEmpoweredAuto = true;
            passiveCooldown = 5;
        }
    }
}