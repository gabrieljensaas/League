using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Riven : ChampionCombat
{
    public static float GetRivenPassiveDamagePercentByLevel(int level)
    {
        if (level == 1) return 30f;
        if (level < 7) return 36f;
        if (level < 10) return 42f;
        if (level < 13) return 48f;
        if (level < 16) return 54f;
        return 60f;
    }

    private int qCounter = 0;
    private float timeSinceLastQ = 0f;
    private float timeSinceR = 0f;
    private float hTimeSinceR = 0f;
    private bool hasWindSlash = false;
    private bool hHasWindSlash = false;
    private CheckIfExecutes r1ExecuteCheck;
    private CheckIfExecutes hR1ExecuteCheck;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "E", "Q", "A" };

        checksQ.Add(new CheckIfCasting(this));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        autoattackcheck = new RivenAACheck(this);
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksQ.Add(new CheckIfImmobilize(this));
        checksE.Add(new CheckIfImmobilize(this));
        r1ExecuteCheck = new CheckIfExecutes(this, "Riven");
        hR1ExecuteCheck = new CheckIfExecutes(this, "SylasRiven");
        checksA.Add(new CheckIfDisarmed(this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Physical Damage");
        wKeys.Add("Physical Damage");
        eKeys.Add("Shield Strength");
        rKeys.Add("Minimum Physical Damage");

        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        timeSinceLastQ += Time.deltaTime;
        timeSinceR += Time.deltaTime;
        if (timeSinceR > 15) hasWindSlash = false;
        hTimeSinceR += Time.deltaTime;
        if (hTimeSinceR > 15) hHasWindSlash = false;
    }

    public override IEnumerator ExecuteQ()
    {
        if (qCounter == 0 && myStats.qCD > 0) yield break;
        if (timeSinceLastQ < 0.3125f) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        UpdateRivenPassive();
        if (qCounter == 0)
        {
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
            timeSinceLastQ = 0f;
            qCounter++;
        }
        else if (qCounter == 2)
        {
            timeSinceLastQ = 0f;
            qCounter = 0;
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.5f, targetStats.buffManager, myStats.qSkill[0].basic.name));
        }
        else
        {
            timeSinceLastQ = 0f;
            qCounter++;
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.75f, targetStats.buffManager, myStats.wSkill[0].basic.name));
        UpdateRivenPassive();
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.buffManager.shields.Add(myStats.eSkill[0].basic.name, new ShieldBuff(2.5f, myStats.buffManager, myStats.eSkill[0].basic.name, myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats), myStats.eSkill[0].basic.name));
        UpdateRivenPassive();
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        if (hasWindSlash && (r1ExecuteCheck.Control() || timeSinceR > 14))
        {
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[1].basic.castTime));
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[1].UseSkill(2, rKeys[0], myStats, targetStats) * (1 + ((targetStats.maxHealth - targetStats.currentHealth) / targetStats.maxHealth) > 0.75f ? 2 : (targetStats.maxHealth - targetStats.currentHealth) * 2.667f), myStats.rSkill[1].basic.name, SkillDamageType.Phyiscal);
            UpdateRivenPassive();
            myStats.rCD = myStats.rSkill[0].basic.coolDown[2] - timeSinceR;
            hasWindSlash = false;
        }

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        yield return StartCoroutine(StartCastingAbility(0.5f - myStats.rSkill[0].basic.castTime));
        UpdateRivenPassive();
        myStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new AttackDamageBuff(15, myStats.buffManager, myStats.rSkill[0].basic.name, (int)(myStats.AD * 0.2f), myStats.rSkill[0].basic.name));
        hasWindSlash = true;
        timeSinceR = 0f;
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        if (hHasWindSlash && (hR1ExecuteCheck.Control() || hTimeSinceR > 14))
        {
            yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
            UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[1].UseSkill(skillLevel, rKeys[0], targetStats, myStats) * (1 + ((myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth) > 0.75f ? 2 : (myStats.maxHealth - myStats.currentHealth) * 2.667f), myStats.rSkill[1].basic.name, SkillDamageType.Phyiscal);
            targetStats.rCD = (myStats.rSkill[0].basic.coolDown[2] * 2) - timeSinceR;
            hasWindSlash = false;
        }

        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(0.5f - myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new AttackDamageBuff(15, targetStats.buffManager, myStats.rSkill[0].basic.name, (int)(targetStats.AP * 0.12f), myStats.rSkill[0].basic.name));
        hHasWindSlash = true;
        hTimeSinceR = 0f;
    }

    private void UpdateRivenPassive()
    {
        if (myStats.buffManager.buffs.TryGetValue("RunicBlade", out Buff value))
        {
            if (value.value == 3) value.duration = 6;
            else
            {
                value.value++;
                value.duration = 6;
            }
        }
        else myStats.buffManager.buffs.Add("RunicBlade", new RunicBladeBuff(6, myStats.buffManager, myStats.passiveSkill.skillName));
    }
}