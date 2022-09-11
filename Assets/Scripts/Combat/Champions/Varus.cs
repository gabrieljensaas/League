using Simulator.Combat;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Varus : ChampionCombat
{
    private bool qEmpowered;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "Q", "E", "A" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksW.Add(new CheckCD(this, "Q"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        checksQ.Add(new CheckIfCasting(this));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        autoattackcheck = new VarusAACheck(this);

        qKeys.Add("Maximum Physical Damage");
        wKeys.Add("Bonus Magic Damage");
        wKeys.Add("Bonus Magic Damage per Stack");
        eKeys.Add("Physical Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }
    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(1.25f, myStats.buffManager, myStats.qSkill[0].basic.name, "PiercingArrow"));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        StartCoroutine(PiercingArrow());
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        qEmpowered = true;
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        CheckBlightStacks();
        targetStats.buffManager.buffs.Add(myStats.eSkill[0].basic.name, new GrievousWoundsBuff(4, targetStats.buffManager, myStats.eSkill[0].basic.name, 25f, myStats.eSkill[0].basic.name));
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        targetStats.buffManager.buffs.Add("Root", new RootBuff(2, targetStats.buffManager, myStats.rSkill[0].basic.name));
        CheckBlightStacks();
        if (targetStats.buffManager.buffs.TryGetValue("Blight", out Buff value))
        {
            value.value = 3;
            value.duration = 6;
        }
        else
        {
            targetStats.buffManager.buffs.Add("Blight", new BlightBuff(6, targetStats.buffManager, "Varus's Auto Attack", 3));
        }
    }

    private IEnumerator PiercingArrow()
    {
        yield return new WaitForSeconds(1.25f);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        if (qEmpowered)
        {
            targetCombat.TakeDamage((targetStats.maxHealth - targetStats.currentHealth) * Constants.GetVarusWActiveTargetsMissingHealthMultiplier(myStats.level), myStats.wSkill[0].basic.name, SkillDamageType.Spell);
            qEmpowered = false;
        }
        CheckBlightStacks(1.5f);
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }

    private void CheckBlightStacks(float multiplier = 1)
    {
        if (targetStats.buffManager.buffs.TryGetValue("Blight", out Buff value))
        {
            UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[1], multiplier * value.value);
            myStats.qCD -= value.value * myStats.qSkill[0].basic.coolDown[4] * 0.12f;
            myStats.wCD -= value.value * myStats.qSkill[0].basic.coolDown[4] * 0.12f;
            myStats.eCD -= value.value * myStats.qSkill[0].basic.coolDown[4] * 0.12f;
            value.Kill();
        }
    }
}