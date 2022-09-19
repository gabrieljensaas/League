using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Akali : ChampionCombat
{
    public static float[] AkaliPassiveDamageByLevel = { 35, 38, 41, 44, 47, 50, 53, 62, 71, 8, 89, 98, 107, 122, 137, 152, 167, 182 };

    public bool eCast = false;
    private float timeSinceE = 0f;
    public bool rCast = false;
    public bool hRCast = false;
    public float timeSinceR = 0f;
    public float hTimeSinceR = 0f;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "A", "" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        autoattackcheck = new AkaliAACheck(this);
        checksE.Add(new CheckIfImmobilize(this));
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

        qKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Minimum Magic Damage");


        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        yield return base.ExecuteQ();
        AssassinsMark();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceE += Time.deltaTime;
        timeSinceR += Time.deltaTime;
        hTimeSinceR += Time.deltaTime;
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        if (!eCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            StartCoroutine(ShurikenFlip());
            myStats.eCD = 0.4f;
            timeSinceE = 0;
            AssassinsMark();
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
            StopCoroutine(ShurikenFlip());
            myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        if (!rCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
            StartCoroutine(PerfectExecution());
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
            myStats.rCD = 2.5f;
            timeSinceR = 0;
            AssassinsMark();
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
            float multiplier = (targetStats.maxHealth - targetStats.currentHealth) / targetStats.maxHealth * 0.0286f;
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0].UseSkill(2, rKeys[1], myStats, targetStats) * (1 + (multiplier > 2 ? 2 : multiplier)), myStats.rSkill[0].basic.name, SkillDamageType.Spell);
            StopCoroutine(PerfectExecution());
            myStats.rCD = myStats.rSkill[0].basic.coolDown[2] - timeSinceR;
        }
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        if (targetStats.buffManager.HasImmobilize) yield break;
        if (!hRCast)
        {
            yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
            StartCoroutine(HPerfectExecution(skillLevel));
            UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
            targetStats.rCD = 2.5f;
            hTimeSinceR = 0;
        }
        else
        {
            yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
            float multiplier = (myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth * 0.0286f;
            UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0].SylasUseSkill(skillLevel, rKeys[1], targetStats, myStats) * (1 + (multiplier > 2 ? 2 : multiplier)), myStats.rSkill[0].basic.name, SkillDamageType.Spell);
            StopCoroutine(HPerfectExecution(skillLevel));
            targetStats.rCD = (targetStats.rSkill[0].basic.coolDown[skillLevel] - hTimeSinceR) * 2;
        }
    }

    public void AssassinsMark()
    {
        if (!targetStats.buffManager.buffs.TryAdd("AkaliPassiveBuff", new AkaliPassiveBuff(4, targetStats.buffManager, myStats.passiveSkill.skillName)))
        {
            targetStats.buffManager.buffs["AkaliPassiveBuff"].duration = 4;
            myStats.buffManager.buffs.Remove("AkaliPassive");
        }
    }

    public IEnumerator ShurikenFlip()
    {
        eCast = true;
        yield return new WaitForSeconds(3f);
        eCast = false;
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
    }

    public IEnumerator PerfectExecution()
    {
        rCast = true;
        yield return new WaitForSeconds(10f);
        rCast = false;
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2] - timeSinceR;
    }

    public IEnumerator HPerfectExecution(int skillLevel)
    {
        hRCast = true;
        yield return new WaitForSeconds(10f);
        hRCast = false;
        targetStats.rCD = (myStats.rSkill[0].basic.coolDown[skillLevel] - hTimeSinceR) * 2;
    }
}