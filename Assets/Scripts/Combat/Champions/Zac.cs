using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Zac : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "Q", "A" };

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
        checksQ.Add(new CheckIfUnableToAct(this));
        checksE.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Reduced Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;                                             //requires extra research
        myStats.currentHealth *= 0.92f;
        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        Invoke(nameof(GetChunk), 0.5f);
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;
        myStats.currentHealth *= 0.96f;
        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        Invoke(nameof(GetChunk), 0.5f);
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;                                       //can be changed to charged version later
        myStats.currentHealth *= 0.96f;
        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.5f, targetStats.buffManager, myStats.eSkill[0].basic.name));
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.5f, targetStats.buffManager, myStats.eSkill[0].basic.name));
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        Invoke(nameof(GetChunk), 0.5f);
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("UnableToAct", new UnableToActBuff(3f, myStats.buffManager, myStats.rSkill[0].basic.name));
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(1f, targetStats.buffManager, myStats.eSkill[0].basic.name));
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        Invoke(nameof(GetChunk), 0.5f);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        Invoke(nameof(GetChunk), 0.5f);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        Invoke(nameof(GetChunk), 0.5f);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        Invoke(nameof(GetChunk), 0.5f);
    }

    public void GetChunk()
    {
        UpdateTotalHeal(ref hSum, myStats.maxHealth * (0.04f + (0.0075f * (2 + 1))), "Cell Division");             //2 is the level of r
        myStats.wCD -= 1;
    }
}