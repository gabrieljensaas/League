using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Lucian : ChampionCombat
{
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

        myStats.qSkill[0].basic.castTime = Constants.LucianQCastTimeByLevel[myStats.level]; //variable cast time
        passiveMultiplier = Constants.GetLucianPassiveMultiplier(myStats.level);

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        AddLightslinger(myStats.qSkill[0].basic.name);
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys);
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

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
        yield return new WaitForSeconds(0.25f);
        if(myStats.buffManager.buffs.TryGetValue("Lightslinger", out Buff value))
        {
            AutoAttack(passiveMultiplier);
            value.Kill();
        }
    }

    private IEnumerator TheCulling(int waveCount, float interval)
    {
        yield return new WaitForSeconds(interval);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys);
        StartCoroutine(TheCulling(waveCount--, interval));
        AddLightslinger(myStats.qSkill[0].basic.name);
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