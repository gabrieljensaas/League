using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Ekko : ChampionCombat
{
    public static float[] passiveDamageFlat = { 30, 40, 50, 60, 70, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140 };

    private float lostHealthPast4Seconds;
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
        checksE.Add(new CheckIfImmobilize(this));
        checksR.Add(new CheckIfImmobilize(this));
        checkTakeDamageAAPostMitigation.Add(new CheckForLostHealth(this, this));
        checkTakeDamageAAPostMitigation.Add(new CheckIfTargetable(this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckForLostHealth(this, this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckIfTargetable(this));
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Magic Damage");
        qKeys.Add("Magic Damage");
        wKeys.Add("Shield Strength");
        eKeys.Add("Bonus Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Heal");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == 0) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        if (UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile) == float.MinValue) yield break;
        AddZDriveResonance();
        yield return new WaitForSeconds(1.75f);
        if (UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile) != float.MinValue) AddZDriveResonance();
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(3f);
        TargetBuffManager.Add("Stun", new StunBuff(2.25f, TargetBuffManager, WSkill().basic.name));
        UpdateAbilityTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, buffNames: new string[] { "Stun" }), WSkill().basic.name);
        MyBuffManager.shields.Add(WSkill().basic.name, new ShieldBuff(2f, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), WSkill().basic.name));
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, SkillComponentTypes.Dash), ESkill().basic.name);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        attackCooldown = 0;
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        MyBuffManager.Add("Stasis", new StasisBuff(0.5f, MyBuffManager, RSkill().basic.name));
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateTotalHeal(ref hSum, RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats), RSkill().basic.name); //add health lost
        if (UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Dash) != float.MinValue) AddZDriveResonance();
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        if (MyBuffManager.buffs.ContainsKey("PhaseDive"))
        {
            yield return StartCoroutine(StartCastingAbility(0.25f));

            if (AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal, SkillComponentTypes.OnHit | SkillComponentTypes.Blink)).damage != float.MinValue)
            {
                if (myStats.wLevel > 0 && targetStats.PercentMissingHealth <= 0.3f)
                {
                    float damage = (targetStats.maxHealth - targetStats.currentHealth) * (0.03f + (myStats.AP * 0.0003f));
                    UpdateAbilityTotalDamage(ref wSum, 1, new Damage(damage < 15 ? 15 : damage, SkillDamageType.Spell, SkillComponentTypes.ProcDamage), WSkill().basic.name);
                }

                UpdateAbilityTotalDamage(ref eSum, 2, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), SkillDamageType.Spell), ESkill().basic.name);
                AddZDriveResonance();
            }
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(0.1f));
            if (AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal, SkillComponentTypes.OnHit)).damage != float.MinValue && myStats.wLevel > 0 && targetStats.PercentMissingHealth <= 0.3f)
            {

                float damage = (targetStats.maxHealth - targetStats.currentHealth) * (0.03f + (myStats.AP * 0.0003f));
                UpdateAbilityTotalDamage(ref wSum, 1, new Damage(damage < 15 ? 15 : damage, SkillDamageType.Spell, SkillComponentTypes.ProcDamage), WSkill().basic.name);
                AddZDriveResonance();
            }
        }
    }

    public IEnumerator AddLostHealth(float hp)
    {
        lostHealthPast4Seconds += hp;
        yield return new WaitForSeconds(4f);
        lostHealthPast4Seconds -= hp;
    }

    public void AddZDriveResonance()
    {
        if (TargetBuffManager.buffs.TryGetValue("ZDriveResonance", out Buff value))
        {
            if (value.value == 0) return;
            value.value++;
            value.duration = 4;
        }
        else
        {
            TargetBuffManager.Add("ZDriveResonance", new ZDriveResonanceBuff(4, TargetBuffManager, "Z-Drive Resonance"));
        }
    }
}