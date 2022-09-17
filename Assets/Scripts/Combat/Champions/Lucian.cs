using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Lucian : ChampionCombat
{
    public static float[] LucianQCastTimeByLevel = { 0.4f, 0.39f, 0.38f, 0.37f, 0.36f, 0.36f, 0.35f, 0.34f, 0.33f, 0.32f, 0.31f, 0.3f, 0.29f, 0.29f, 0.28f, 0.27f, 0.26f, 0.25f };

    public static float GetLucianPassiveMultiplier(int level)
    {
        if (level < 7) return 0.5f;
        if (level < 13) return 0.55f;
        return 0.6f;
    }

    private float passiveMultiplier;
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
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

        myStats.qSkill[0].basic.castTime = LucianQCastTimeByLevel[myStats.level]; //variable cast time
        passiveMultiplier = GetLucianPassiveMultiplier(myStats.level);

        qKeys.Add("Physical Damage");
        wKeys.Add("Magic Damage");
        rKeys.Add("Physical Damage Per Shot");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        AddLightslinger(myStats.qSkill[0].basic.name);
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        AddLightslinger(myStats.wSkill[0].basic.name);
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        AddLightslinger(myStats.eSkill[0].basic.name);
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(3, myStats.buffManager, myStats.rSkill[0].basic.name, "TheCulling"));
        StartCoroutine(TheCulling(22 + 0, (22 + 0) / 3f));                // +0 needs to be added later as critical chance
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        AddLightslinger(myStats.rSkill[0].basic.name);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(3, targetStats.buffManager, myStats.rSkill[0].basic.name, "HTheCulling"));
        StartCoroutine(HTheCulling(22 + 0, (22 + 0) / 3f, skillLevel));                // +0 needs to be added later as critical chance
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
        yield return new WaitForSeconds(0.25f);
        if (myStats.buffManager.buffs.TryGetValue("Lightslinger", out Buff value))
        {
            AutoAttack(passiveMultiplier);
            myStats.eCD -= 2f;
            value.Kill();
        }
    }

    private IEnumerator TheCulling(int waveCount, float interval)
    {
        if (waveCount == 0) yield break;
        yield return new WaitForSeconds(interval);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        StartCoroutine(TheCulling(waveCount--, interval));
        AddLightslinger(myStats.qSkill[0].basic.name);
    }

    private IEnumerator HTheCulling(int waveCount, float interval, int skillLevel)
    {
        if (waveCount == 0) yield break;
        yield return new WaitForSeconds(interval);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        StartCoroutine(HTheCulling(waveCount--, interval, skillLevel));
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }

    private void AddLightslinger(string source)
    {
        if (myStats.buffManager.buffs.TryGetValue("Lightslinger", out Buff value))
        {
            value.duration = 3.5f;
        }
        else
        {
            myStats.buffManager.buffs.Add("Lightslinger", new LightslingerBuff(3.5f, myStats.buffManager, source));
        }
    }
}