using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class MissFortune : ChampionCombat
{
    private bool loveTapped = false;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "R", "W", "Q", "A" };

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
        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        if (!loveTapped) LoveTap();
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        StartCoroutine(MakeItRain());
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(3, myStats.buffManager, myStats.rSkill[0].basic.name, "BulletTime"));
        StartCoroutine(BulletTime((int)Constants.MissFortuneRWaveCountBySkillLevel[2]));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
        if (!loveTapped) LoveTap();
    }
    private void LoveTap()
    {
        UpdateAbilityTotalDamage(ref pSum, 4, myStats.AD * Constants.GetMissfortunePassiveADMultiplier(myStats.level), myStats.passiveSkill.skillName, SkillDamageType.Phyiscal);
        myStats.wCD -= 2;
        loveTapped = true;
    }

    private IEnumerator MakeItRain()
    {
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys, 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys, 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys, 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys, 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys, 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys, 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys, 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys, 0.125f);
    }

    private IEnumerator BulletTime(int waveCount)
    {
        yield return new WaitForSeconds(Constants.MissFortuneRWaveIntervalTimeBySkillLevel[2]);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys);
        StartCoroutine(BulletTime(waveCount--));
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }
}