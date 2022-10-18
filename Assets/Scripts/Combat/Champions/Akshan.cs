using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Akshan : ChampionCombat
{
    private float pCD = 0;

    public static float[] passiveDamage = { 10, 15, 20, 25, 30, 35, 40, 45, 55, 65, 75, 85, 95, 105, 120, 135, 150, 165 };
    public static float GetPassiveCD(float level)
    {
        return level switch
        {
            < 6 => 16,
            < 11 => 12,
            < 16 => 8,
            _ => 4
        };
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "", "E", "Q", "A" };

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
        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));

        qKeys.Add("Physical Damage");
        eKeys.Add("Physical Damage per Shot");
        rKeys.Add("Maximum Bullets Stored");
        rKeys.Add("Minimum Physical Damage per Bullet");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        pCD -= Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if (UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable) != float.MinValue)
        {
            if (MyBuffManager.buffs.TryGetValue("DirtyFighting", out Buff value))
            {
                value.duration = 5f;
                value.value++;
                if (value.value >= 3)
                {
                    UpdateAbilityTotalDamage(ref pSum, 5, new Damage(passiveDamage[myStats.level], SkillDamageType.Spell), "Dirty Fighting");
                    value.Kill();
                    if (pCD <= 0)
                    {
                        MyBuffManager.shields.Add("DirtyFighting", new ShieldBuff(2, MyBuffManager, "Dirty Fighting", 40 + (240 / 17 * (myStats.level - 1)) + (myStats.bonusAD * 0.35f), "DirtyFighting"));
                        pCD = GetPassiveCD(myStats.level);
                    }
                }
            }
            else
            {
                MyBuffManager.Add("DirtyFighting", new DirtyFightingBuff(5, MyBuffManager, "Akshan's Auto Attack"));
            }
        }
        yield return new WaitForSeconds(0.65f);
        if (UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable) != float.MinValue)
        {
            if (MyBuffManager.buffs.TryGetValue("DirtyFighting", out Buff value))
            {
                value.duration = 5f;
                value.value++;
                if (value.value >= 3)
                {
                    UpdateAbilityTotalDamage(ref pSum, 5, new Damage(passiveDamage[myStats.level], SkillDamageType.Spell), "Dirty Fighting");
                    value.Kill();
                    if (pCD <= 0)
                    {
                        MyBuffManager.shields.Add("DirtyFighting", new ShieldBuff(2, MyBuffManager, "Dirty Fighting", 40 + (240 / 17 * (myStats.level - 1)) + (myStats.bonusAD * 0.35f), "DirtyFighting"));
                        pCD = GetPassiveCD(myStats.level);
                    }
                }
            }
            else
            {
                MyBuffManager.Add("DirtyFighting", new DirtyFightingBuff(5, MyBuffManager, "Akshan's Auto Attack"));
            }
        }
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        if (UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: SkillComponentTypes.Dash | SkillComponentTypes.Projectile | SkillComponentTypes.OnHit | SkillComponentTypes.Spellblockable | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable) != float.MinValue)
        {
            if (MyBuffManager.buffs.TryGetValue("DirtyFighting", out Buff buff))
            {
                buff.duration = 5f;
                buff.value++;
                if (buff.value >= 3)
                {
                    UpdateAbilityTotalDamage(ref pSum, 5, new Damage(passiveDamage[myStats.level], SkillDamageType.Spell), "Dirty Fighting");
                    buff.Kill();
                    buff.Kill();
                    if (pCD <= 0)
                    {
                        MyBuffManager.shields.Add("DirtyFighting", new ShieldBuff(2, MyBuffManager, "Dirty Fighting", 40 + (240 / 17 * (myStats.level - 1)) + (myStats.bonusAD * 0.35f), "DirtyFighting"));
                        pCD = GetPassiveCD(myStats.level);
                    }
                }
            }
            else
            {
                MyBuffManager.Add("DirtyFighting", new DirtyFightingBuff(5, MyBuffManager, "Akshan's Auto Attack"));
            }
        }
        StartCoroutine(HeroicSwing(13));
        MyBuffManager.Add("Channeling", new ChannelingBuff(3f, MyBuffManager, ESkill().basic.name, "HeroicSwing"));
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (targetStats.PercentCurrentHealth > 0.5) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        StartCoroutine(Comeuppance(RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats)));
        MyBuffManager.Add("Channeling", new ChannelingBuff(2.5f, MyBuffManager, RSkill().basic.name, "Comeuppance"));
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        if (myStats.PercentCurrentHealth > 0.5) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(RSkill().basic.castTime));
        StartCoroutine(HComeuppance(RSkill().UseSkill(skillLevel, rKeys[0], targetStats, myStats)));
        TargetBuffManager.Add("Channeling", new ChannelingBuff(2.5f, MyBuffManager, RSkill().basic.name, "Comeuppance"));
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal, SkillComponentTypes.OnHit | SkillComponentTypes.Projectile | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable)).damage != float.MinValue)
        {
            if (MyBuffManager.buffs.TryGetValue("DirtyFighting", out Buff value))
            {
                value.duration = 5f;
                value.value++;
                if (value.value >= 3)
                {
                    UpdateAbilityTotalDamage(ref pSum, 5, new Damage(passiveDamage[myStats.level], SkillDamageType.Spell), "Dirty Fighting");
                    value.Kill();
                    if (pCD <= 0)
                    {
                        MyBuffManager.shields.Add("DirtyFighting", new ShieldBuff(2, MyBuffManager, "Dirty Fighting", 40 + (240 / 17 * (myStats.level - 1)) + (myStats.bonusAD * 0.35f), "DirtyFighting"));
                        pCD = GetPassiveCD(myStats.level);
                    }
                }
            }
            else
            {
                MyBuffManager.Add("DirtyFighting", new DirtyFightingBuff(5, MyBuffManager, "Akshan's Auto Attack"));
            }
        }
        if (AutoAttack(new Damage(myStats.AD * 0.5f, SkillDamageType.Phyiscal, SkillComponentTypes.OnHit | SkillComponentTypes.Projectile | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable)).damage != float.MinValue)
        {
            if (MyBuffManager.buffs.TryGetValue("DirtyFighting", out Buff buff))
            {
                buff.duration = 5f;
                buff.value++;
                if (buff.value >= 3)
                {
                    UpdateAbilityTotalDamage(ref pSum, 5, new Damage(passiveDamage[myStats.level], SkillDamageType.Spell), "Dirty Fighting");
                    buff.Kill();
                    buff.Kill();
                    if (pCD <= 0)
                    {
                        MyBuffManager.shields.Add("DirtyFighting", new ShieldBuff(2, MyBuffManager, "Dirty Fighting", 40 + (240 / 17 * (myStats.level - 1)) + (myStats.bonusAD * 0.35f), "DirtyFighting"));
                        pCD = GetPassiveCD(myStats.level);
                    }
                }
            }
            else
            {
                MyBuffManager.Add("DirtyFighting", new DirtyFightingBuff(5, MyBuffManager, "Akshan's Auto Attack"));
            }
        }
    }

    public IEnumerator HeroicSwing(int shots)
    {
        if (shots > 0)
        {
            yield return new WaitForSeconds(0.23f);
            if (UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: SkillComponentTypes.Dash | SkillComponentTypes.Projectile | SkillComponentTypes.OnHit | SkillComponentTypes.Spellblockable | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable) != float.MinValue)
            {
                if (MyBuffManager.buffs.TryGetValue("DirtyFighting", out Buff buff))
                {
                    buff.duration = 5f;
                    buff.value++;
                    if (buff.value >= 3)
                    {
                        UpdateAbilityTotalDamage(ref pSum, 5, new Damage(passiveDamage[myStats.level], SkillDamageType.Spell), "Dirty Fighting");
                        buff.Kill();
                        buff.Kill();
                        if (pCD <= 0)
                        {
                            MyBuffManager.shields.Add("DirtyFighting", new ShieldBuff(2, MyBuffManager, "Dirty Fighting", 40 + (240 / 17 * (myStats.level - 1)) + (myStats.bonusAD * 0.35f), "DirtyFighting"));
                            pCD = GetPassiveCD(myStats.level);
                        }
                    }
                }
                else
                {
                    MyBuffManager.Add("DirtyFighting", new DirtyFightingBuff(5, MyBuffManager, "Akshan's Auto Attack"));
                }
            }
            StartCoroutine(HeroicSwing(shots - 1));
        }
        else
        {
            if (UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: SkillComponentTypes.Dash | SkillComponentTypes.Projectile | SkillComponentTypes.OnHit | SkillComponentTypes.Spellblockable | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable) != float.MinValue)
            {
                if (MyBuffManager.buffs.TryGetValue("DirtyFighting", out Buff buff))
                {
                    buff.duration = 5f;
                    buff.value++;
                    if (buff.value >= 3)
                    {
                        UpdateAbilityTotalDamage(ref pSum, 5, new Damage(passiveDamage[myStats.level], SkillDamageType.Spell), "Dirty Fighting");
                        buff.Kill();
                        buff.Kill();
                        if (pCD <= 0)
                        {
                            MyBuffManager.shields.Add("DirtyFighting", new ShieldBuff(2, MyBuffManager, "Dirty Fighting", 40 + (240 / 17 * (myStats.level - 1)) + (myStats.bonusAD * 0.35f), "DirtyFighting"));
                            pCD = GetPassiveCD(myStats.level);
                        }
                    }
                }
                else
                {
                    MyBuffManager.Add("DirtyFighting", new DirtyFightingBuff(5, MyBuffManager, "Akshan's Auto Attack"));
                }
            }
            myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        }
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
        if ("HeroicSwing" == uniqueKey) myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        if ("Comeuppance" == uniqueKey) myStats.rCD = 5f;
    }

    public IEnumerator Comeuppance(float bullets)
    {
        yield return new WaitForSeconds(2.5f);
        StartCastingAbility(bullets * 0.1f);
        while (bullets > 0)
        {
            yield return new WaitForSeconds(0.1f);
            if (UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[1], damageModifier: 1 + (targetStats.PercentMissingHealth * 3), skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable) != float.MinValue)
            {
                if (MyBuffManager.buffs.TryGetValue("DirtyFighting", out Buff value))
                {
                    value.duration = 5f;
                    value.value++;
                    if (value.value >= 3)
                    {
                        UpdateAbilityTotalDamage(ref pSum, 5, new Damage(passiveDamage[myStats.level], SkillDamageType.Spell), "Dirty Fighting");
                        value.Kill();
                        if (pCD <= 0)
                        {
                            MyBuffManager.shields.Add("DirtyFighting", new ShieldBuff(2, MyBuffManager, "Dirty Fighting", 40 + (240 / 17 * (myStats.level - 1)) + (myStats.bonusAD * 0.35f), "DirtyFighting"));
                            pCD = GetPassiveCD(myStats.level);
                        }
                    }
                }
                else
                {
                    MyBuffManager.Add("DirtyFighting", new DirtyFightingBuff(5, MyBuffManager, "Akshan's Auto Attack"));
                }
            }
        }
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public IEnumerator HComeuppance(float bullets)
    {
        yield return new WaitForSeconds(2.5f);
        targetCombat.StartCastingAbility(bullets * 0.1f);
        while (bullets > 0)
        {
            yield return new WaitForSeconds(0.1f);
            targetCombat.UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, RSkill(), targetStats.rLevel, rKeys[1], damageModifier: 1 + (myStats.PercentMissingHealth * 3), skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable);
        }
        targetStats.rCD = RSkill().basic.coolDown[targetStats.rLevel] * 2;
    }
}