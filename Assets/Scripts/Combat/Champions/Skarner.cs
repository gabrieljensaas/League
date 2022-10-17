using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Skarner : ChampionCombat
{
    private bool isCharged = false;
    public bool hasCrystalVenom;
    public bool hasImpale;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "Q", "W", "R", "A" };

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
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Physical Damage");
        qKeys.Add("Bonus Magic Damage");
        wKeys.Add("Shield Strength");
        eKeys.Add("Magic Damage");
        eKeys.Add("Bonus Physical Damage");
        rKeys.Add("Total Mixed Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
        if (isCharged) UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[1]);
        isCharged = true;
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        MyBuffManager.Add("ShieldBuff", new ShieldBuff(5, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), "ExoSkeleton"));
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || hasImpale) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
        TargetBuffManager.Add("CrystalVenom", new CrystalVenomBuff(5, TargetBuffManager, ESkill().basic.name));
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        TargetBuffManager.Add("RootBuff", new RootBuff(RSkill().basic.castTime, TargetBuffManager, ESkill().basic.name));
        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        TargetBuffManager.Add("ImpaleVenom", new ImpaleBuff(5, TargetBuffManager, ESkill().basic.name));
        UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), SkillDamageType.PhysAndSpell), RSkill().basic.name);
        TargetBuffManager.Add("SuppressionBuff", new SuppressionBuff(1.75f, TargetBuffManager, ESkill().basic.name));
        MyBuffManager.Add("CantAA", new CantAABuff(2f, MyBuffManager, RSkill().basic.name));
        myStats.rCD = RSkill().basic.coolDown[2];
        myStats.eCD -= 2f;
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if (hasCrystalVenom)
        {
            UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[1]);
            TargetBuffManager.Add("StunBuff", new StunBuff(1.25f, TargetBuffManager, ESkill().basic.name));
            myStats.eCD -= 1.25f;
        }
    }
}
public class CrystalVenomBuff : Buff
{
    private readonly Skarner skarner;
    public CrystalVenomBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Crystal Venom For {duration} Seconds!");
        skarner.hasCrystalVenom = true;
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Crystal Venom Ended!");
        skarner.hasCrystalVenom = false;
        manager.buffs.Remove("CrystalVenom");
    }
}
public class ImpaleBuff : Buff
{
    private readonly Skarner skarner;
    public ImpaleBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Crystal Venom For {duration} Seconds!");
        skarner.hasImpale = true;
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Crystal Venom Ended!");
        skarner.hasImpale = false;
        manager.buffs.Remove("CrystalVenom");
    }
}