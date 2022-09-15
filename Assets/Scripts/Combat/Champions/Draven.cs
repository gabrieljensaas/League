using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Draven : ChampionCombat
{
    private int pStack = 1;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "W", "E", "A" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        checksQ.Add(new CheckIfCasting(this));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfExecutes(this, "R", pStack, 2));
        checksA.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));
        autoattackcheck = new DravenAACheck(this, this);

        qKeys.Add("Bonus Physical Damage");
        wKeys.Add("Bonus Attack Speed");
        eKeys.Add("Physical Damage");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if (myStats.buffManager.buffs.TryGetValue("SpinningAxe", out Buff value))
        {
            if (value.value != 2) value.value++;
            value.duration = 5.8f;
        }
        else
        {
            myStats.buffManager.buffs.Add("SpinningAxe", new SpinningAxeBuff(5.8f, myStats.buffManager, myStats.qSkill[0].basic.name));
        }
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add(myStats.wSkill[0].basic.name, new AttackSpeedBuff(3, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.1f, targetStats.buffManager, myStats.eSkill[0].basic.name));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        if (targetStats.currentHealth <= pStack) targetStats.currentHealth -= pStack;
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        if (targetStats.currentHealth <= pStack) targetStats.currentHealth -= pStack;
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
    }

    public IEnumerator SpinnigAxe()
    {
        yield return new WaitForSeconds(2);
        myStats.wCD = -0.1f;
        if (myStats.buffManager.buffs.TryGetValue("SpinningAxe", out Buff value))
        {
            value.value++;
            value.duration = 5.8f;
        }
        else
        {
            myStats.buffManager.buffs.Add("SpinningAxe", new SpinningAxeBuff(5.8f, myStats.buffManager, myStats.qSkill[0].basic.name));
        }
    }
}