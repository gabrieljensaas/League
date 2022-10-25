using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Karthus : ChampionCombat
{
    private bool eCast = false;
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
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));
        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));

        qKeys.Add("Enhanced Damage");
        eKeys.Add("Magic Damage Per Tick");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(0.6f);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        MyBuffManager.buffs.Add("MRReduction", new MagicResistanceReductionBuff(4, TargetBuffManager, WSkill(0).basic.name, 0.15f, "MRReduction"));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;
        if (eCast) yield break;
        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        eCast = !eCast;
        StartCoroutine(Defile());
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(3f, MyBuffManager, RSkill(0).basic.name, "Requiem"));
        StartCoroutine(Requiem());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public IEnumerator Defile()
    {

        yield return new WaitForSeconds(0.25f);
        UpdateTotalDamage(ref eSum, 2, ESkill(0), 4, eKeys[0]);
        StartCoroutine(Defile());
    }

    public IEnumerator Requiem()
    {
        yield return new WaitForSeconds(3f);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }
}