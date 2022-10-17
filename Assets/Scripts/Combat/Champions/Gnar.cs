using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Gnar : ChampionCombat
{
    public static float[] MiniASBonus = { 5.5f, 9.46f, 13.61f, 17, 96f, 22.5f, 27.23f, 32.15f, 37.26f, 42.57f, 48.07f, 53.76f, 59.65f, 65.72f, 72f, 78.46f, 85.11f, 91.96f, 99f };
    public static float[] MegaHPBonus = { 100, 130.96f, 163.43f, 197.4f, 232.87f, 269.85f, 308.33f, 348.33f, 389.82f, 432.82f, 477.33f, 523.34f, 570.85f, 619.87f, 670.4f, 722.43f, 775.96f, 831f };
    public static float[] MegaArmorBonus = { 3.5f, 5.66f, 7.93f, 10.3f, 12.77f, 15.35f, 18.04f, 20.83f, 23.72f, 26.72f, 29.83f, 33.03f, 36.35f, 39.77f, 43.3f, 46.93f, 50.66f, 54.5f };
    public static float[] MegaSpellBlockBonus = { 3.5f, 6.02f, 8.66f, 11.43f, 14.32f, 17.33f, 20.46f, 23.71f, 27.09f, 30.59f, 34.21f, 37.96f, 41.83f, 45.82f, 49.93f, 54.16f, 58.52f, 63 };
    public static float[] MegaBonusAD = { 8, 9.8f, 11.69f, 13.66f, 15.73f, 17.88f, 20.11f, 22.44f, 24.85f, 27.35f, 29.94f, 32.61f, 35.38f, 38.23f, 41.16f, 44.19f, 47.3f, 50.5f };
    public bool IsMini = true;
    public float HyperTimer = 0f;
    public int HyperCount = 0;
    public float RageBar = 0;
    public bool Transformed = false;
    public bool CantRage = false;

    public float GetGeneratedRageByLevel(int level)
    {
        return level switch
        {
            < 6 => 4,
            < 11 => 7,
            _ => 11
        };
    }

    public float GetQGeneratedRageByLevel(int level)
    {
        return level switch
        {
            < 6 => 1,
            < 11 => 1.75f,
            _ => 2.75f
        };
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "E", "Q", "A" };

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
        checkTakeDamageAAPostMitigation.Add(new CheckForGnarRage(this, this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckForGnarRage(this, this));

        qKeys.Add("Physical Damage");
        qKeys.Add("Physical Damage");
        wKeys.Add("Bonus Magic Damage");
        wKeys.Add("Physical Damage");
        eKeys.Add("Bonus Attack Speed");
        eKeys.Add("Physical Damage");
        eKeys.Add("Physical Damage");
        rKeys.Add("Increased Damage");
        rKeys.Add("Disable Duration");

        attackCooldown *= myStats.attackSpeed / myStats.attackSpeed + (myStats.baseAttackSpeed * MiniASBonus[myStats.level] * 0.01f);
        myStats.attackSpeed += myStats.baseAttackSpeed * MiniASBonus[myStats.level] * 0.01f;

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        HyperTimer += Time.fixedDeltaTime;
        if (HyperTimer > 3.5f)
        {
            HyperTimer = 0f;
            HyperCount = 0;
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        if (IsMini)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
            StopCoroutine(GenerateRage());
            StartCoroutine(GenerateRage());
            if (!CantRage) RageBar = RageBar + GetQGeneratedRageByLevel(myStats.level) > 100 ? 100 : RageBar + GetQGeneratedRageByLevel(myStats.level);
            if (IsMini) HyperCount++;
            if (HyperCount == 3)
            {
                UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
                StopCoroutine(GenerateRage());
                StartCoroutine(GenerateRage());
                HyperCount = 0;
            }
            HyperTimer = 0;
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4] * 0.6f;
            if (RageBar == 100)
            {
                IsMini = false;
                StartCoroutine(Transform());
            }
        }
        else
        {
            if (!Transformed)
            {
                Transformed = true;
                TurnToMega();
            }
            yield return StartCoroutine(StartCastingAbility(myStats.qSkill[1].basic.castTime));
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[1], 4, qKeys[1]);
            myStats.qCD = myStats.qSkill[1].basic.coolDown[4] * 0.3f;
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;
        if (IsMini) yield break;
        if (!Transformed)
        {
            Transformed = true;
            TurnToMega();
        }
        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[1].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[1], 4, wKeys[1]);
        myStats.wCD = myStats.wSkill[1].basic.coolDown[4];
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(1.25f, targetStats.buffManager, myStats.wSkill[1].basic.name));
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        if (IsMini)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            myStats.buffManager.buffs.Add(myStats.eSkill[0].basic.name, new AttackSpeedBuff(6, myStats.buffManager, myStats.eSkill[0].basic.name, myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats), myStats.eSkill[0].basic.name));
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
            StopCoroutine(GenerateRage());
            StartCoroutine(GenerateRage());
            if (IsMini) HyperCount++;
            if (HyperCount == 3)
            {
                UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
                StopCoroutine(GenerateRage());
                StartCoroutine(GenerateRage());
                HyperCount = 0;
            }
            HyperTimer = 0;
            myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        }
        else
        {
            if (!Transformed)
            {
                Transformed = true;
                TurnToMega();
            }
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[1].basic.castTime));
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[1], 4, eKeys[2]);
            myStats.eCD = myStats.eSkill[1].basic.coolDown[4];
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;
        if (IsMini) yield break;
        if (!Transformed)
        {
            Transformed = true;
            TurnToMega();
        }
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.4f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        yield return new WaitForSeconds(0.4f);
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(myStats.rSkill[0].UseSkill(2, rKeys[1], myStats, targetStats), targetStats.buffManager, myStats.rSkill[0].basic.name));
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (IsMini) HyperCount++;
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        StopCoroutine(GenerateRage());
        StartCoroutine(GenerateRage());
        if (!CantRage) RageBar = RageBar + (2 * GetQGeneratedRageByLevel(myStats.level)) > 100 ? 100 : RageBar + (GetQGeneratedRageByLevel(myStats.level) * 2);
        if (HyperCount == 3)
        {
            UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
            StopCoroutine(GenerateRage());
            StartCoroutine(GenerateRage());
            HyperCount = 0;
        }
        HyperTimer = 0;
        if (RageBar == 100)
        {
            IsMini = false;
            StartCoroutine(Transform());
        }
    }

    public void TurnToMini()
    {
        attackCooldown *= myStats.attackSpeed / myStats.attackSpeed + (myStats.baseAttackSpeed * MiniASBonus[myStats.level] * 0.01f);
        myStats.attackSpeed += myStats.baseAttackSpeed * MiniASBonus[myStats.level] * 0.01f;
        myStats.baseHealth -= MegaHPBonus[myStats.level];
        float percentage = myStats.currentHealth / myStats.maxHealth;
        myStats.maxHealth -= MegaHPBonus[myStats.level];
        myStats.currentHealth = myStats.maxHealth * percentage;
        myStats.baseArmor -= MegaArmorBonus[myStats.level];
        myStats.armor -= MegaArmorBonus[myStats.level];
        myStats.baseSpellBlock -= MegaSpellBlockBonus[myStats.level];
        myStats.spellBlock -= MegaSpellBlockBonus[myStats.level];
        myStats.baseAD -= MegaBonusAD[myStats.level];
        myStats.AD -= MegaBonusAD[myStats.level];
    }

    public void TurnToMega()
    {
        attackCooldown *= myStats.attackSpeed / myStats.attackSpeed - (myStats.baseAttackSpeed * MiniASBonus[myStats.level] * 0.01f);
        myStats.attackSpeed -= myStats.baseAttackSpeed * MiniASBonus[myStats.level] * 0.01f;
        myStats.baseHealth += MegaHPBonus[myStats.level];
        myStats.maxHealth += MegaHPBonus[myStats.level];
        myStats.currentHealth += MegaHPBonus[myStats.level];
        myStats.baseArmor += MegaArmorBonus[myStats.level];
        myStats.armor += MegaArmorBonus[myStats.level];
        myStats.baseSpellBlock += MegaSpellBlockBonus[myStats.level];
        myStats.spellBlock += MegaSpellBlockBonus[myStats.level];
        myStats.baseAD += MegaBonusAD[myStats.level];
        myStats.AD += MegaBonusAD[myStats.level];
    }

    public IEnumerator GenerateRage()
    {
        float rage = GetGeneratedRageByLevel(myStats.level) * 0.25f;
        yield return new WaitForSeconds(0.5f);
        if (!CantRage) RageBar = RageBar + rage > 100 ? 100 : RageBar + rage;
        if (RageBar == 100)
        {
            IsMini = false;
            StartCoroutine(Transform());
        }
        yield return new WaitForSeconds(0.5f);
        if (!CantRage) RageBar = RageBar + rage > 100 ? 100 : RageBar + rage;
        if (RageBar == 100)
        {
            IsMini = false;
            StartCoroutine(Transform());
        }
        yield return new WaitForSeconds(0.5f);
        if (!CantRage) RageBar = RageBar + rage > 100 ? 100 : RageBar + rage;
        if (RageBar == 100)
        {
            IsMini = false;
            StartCoroutine(Transform());
        }
        yield return new WaitForSeconds(0.5f);
        if (!CantRage) RageBar = RageBar + rage > 100 ? 100 : RageBar + rage;
        if (RageBar == 100)
        {
            IsMini = false;
            StartCoroutine(Transform());
        }
    }

    public IEnumerator Transform()
    {
        yield return new WaitForSeconds(4f);
        Transformed = true;
    }

    public IEnumerator TransformBack()
    {
        yield return new WaitForSeconds(15f);
        Transformed = false;
        IsMini = true;
        RageBar = 0;
        CantRage = true;
        TurnToMini();
        yield return new WaitForSeconds(15f);
        CantRage = false;
    }
}