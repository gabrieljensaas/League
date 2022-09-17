using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Akali : ChampionCombat
{
    public static float[] AkaliPassiveDamageByLevel = { 35, 38, 41, 44, 47, 50, 53, 62, 71, 8, 89, 98, 107, 122, 137, 152, 167, 182 };
    public static float GetAkaliR2Damage(float targetMissingHealth)
    { 
        return targetMissingHealth switch
        {
            < 7 => 0f,
            < 14 => 0.2f,
            < 21 => 0.4f,
            < 28 => 0.6f,
            < 35 => 0.8f,
            < 42 => 1f,
            < 49 => 1.2f,
            < 56 => 1.4f,
            < 63 => 1.6f,
            < 70 => 1.8f,
            _ => 2f
        };
    }

    public bool eCast = false;
    private float timeSinceE = 0f;
    public bool rCast = false;
    public float timeSinceR = 0f;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "Q", "W", "R", "A" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        autoattackcheck = new AkaliAACheck(this);
        checksA.Add(new CheckIfTotalCC(this));
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

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
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[1].basic.castTime));
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
            myStats.rCD = 0.4f;
            timeSinceR = 0;
            AssassinsMark();
        }
        else
        {
            yield return new WaitForSeconds(2.5f);
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[1].basic.castTime));
            UpdateAbilityTotalDamage(ref rSum,0, GetAkaliR2Damage((targetStats.maxHealth - targetStats.currentHealth) / targetStats.maxHealth * 0.01f), myStats.rSkill[0].basic.name, SkillDamageType.Spell);
            StopCoroutine(PerfectExecution());
            myStats.rCD = myStats.rSkill[0].basic.coolDown[4] - timeSinceR;
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
        myStats.rCD = myStats.rSkill[0].basic.coolDown[4] - timeSinceR;
    }
}