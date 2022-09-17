using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class MissFortune : ChampionCombat
{
    public static float[] MissFortuneRWaveIntervalTimeBySkillLevel = { 0.2036f, 0.1781f, 0.1583f };
    public static float[] MissFortuneRWaveCountBySkillLevel = { 14, 16, 18 };

    public static float GetMissfortunePassiveADMultiplier(int level)
    {
        if (level < 4) return 0.5f;
        if (level < 7) return 0.6f;
        if (level < 9) return 0.7f;
        if (level < 11) return 0.8f;
        if (level < 13) return 0.9f;
        return 1;
    }

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
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Physical Damage");
        wKeys.Add("Bonus Attack Speed");
        eKeys.Add("Magic Damage Per Tick");
        rKeys.Add("Total Waves");
        rKeys.Add("Wave Interval Time");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        if (!loveTapped) LoveTap();
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add(myStats.wSkill[0].basic.name, new AttackSpeedBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
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
        StartCoroutine(BulletTime((int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats)));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(3, targetStats.buffManager, myStats.rSkill[0].basic.name, "HBulletTime"));
        StartCoroutine(HBulletTime((int)myStats.rSkill[0].SylasUseSkill(skillLevel, rKeys[0], targetStats, myStats), skillLevel));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
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
        UpdateAbilityTotalDamage(ref pSum, 4, myStats.AD * GetMissfortunePassiveADMultiplier(myStats.level), myStats.passiveSkill.skillName, SkillDamageType.Phyiscal);
        myStats.wCD -= 2;
        loveTapped = true;
    }

    private IEnumerator MakeItRain()
    {
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0], 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0], 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0], 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0], 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0], 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0], 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0], 0.125f);
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0], 0.125f);
    }

    private IEnumerator BulletTime(int waveCount)
    {
        if (waveCount == 0) yield break;
        yield return new WaitForSeconds(myStats.rSkill[0].UseSkill(2, rKeys[1], myStats, targetStats));
        UpdateAbilityTotalDamage(ref rSum, 3, (myStats.AD * 0.75f) + (myStats.AP * 0.2f), "Bullet Time", SkillDamageType.Phyiscal);
        StartCoroutine(BulletTime(waveCount--));
    }

    private IEnumerator HBulletTime(int waveCount, int skillLevel)
    {
        if (waveCount == 0) yield break;
        yield return new WaitForSeconds(myStats.rSkill[0].SylasUseSkill(skillLevel, rKeys[1], targetStats, myStats));
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, (myStats.AP * 0.45f) + (myStats.AP * 0.2f), "Bullet Time", SkillDamageType.Phyiscal);
        StartCoroutine(HBulletTime(waveCount--, skillLevel));
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }
}