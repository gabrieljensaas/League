using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Viktor : ChampionCombat
{
    private bool qAugmented = true;
    private bool eAugmented = true;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "Q", "E", "A" };

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
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Discharge Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("Aftershock Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage Per Tick");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if (!qAugmented) myStats.buffManager.shields.Add(myStats.qSkill[0].basic.name, new ShieldBuff(2.5f, myStats.buffManager, myStats.qSkill[0].basic.name, 27 + (78 / 17 * (myStats.level - 1)) + (myStats.AP * 0.18f), myStats.qSkill[0].basic.name));
        else myStats.buffManager.shields.Add(myStats.qSkill[0].basic.name, new ShieldBuff(2.5f, myStats.buffManager, myStats.qSkill[0].basic.name, 40 + (8 * myStats.level) + (myStats.AP * 0.32f), myStats.qSkill[0].basic.name));
        myStats.buffManager.buffs.Add("Discharge", new DischargeBuff(3.5f, myStats.buffManager, myStats.qSkill[0].basic.name));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(2.25f);
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(1.5f, targetStats.buffManager, myStats.wSkill[0].basic.name));
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        if (eAugmented)
        {
            yield return new WaitForSeconds(1f);
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Disrupt", new DisruptBuff(0, targetStats.buffManager, myStats.rSkill[0].basic.name));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        StartCoroutine(ChaosStorm());
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Disrupt", new DisruptBuff(0, myStats.buffManager, myStats.rSkill[0].basic.name));
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
        StartCoroutine(HChaosStorm(skillLevel));
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (myStats.buffManager.buffs.TryGetValue("Discharge", out Buff value))
        {
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[1]);
            value.Kill();
        }
        else
        {
            AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        }
    }

    public IEnumerator ChaosStorm()
    {
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
    }

    public IEnumerator HChaosStorm(int skillLevel)
    {
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
    }
}