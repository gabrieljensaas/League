using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class KogMaw : ChampionCombat
{
    public static float[] KogMawQReductionBySkillLevel = { 23, 25, 27, 29, 31 };

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
        autoattackcheck = new KogMawAACheck(this);
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));

        //add q passive bonus here

        qKeys.Add("Magic Damage");
        qKeys.Add("Resistances Reduction");
        wKeys.Add("Bonus Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        targetStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new ArmorReductionBuff(4, targetStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(4, qKeys[1], myStats, targetStats), myStats.qSkill[0].basic.name));
        targetStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new MagicResistanceReductionBuff(4, targetStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(4, qKeys[1], myStats, targetStats), myStats.qSkill[0].basic.name));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }
    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("BioArcaneBarrage", new BioArcaneBarrageBuff(8, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats)));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        float multiplier;
        if ((targetStats.maxHealth - targetStats.currentHealth) / targetStats.maxHealth > 0.6f) multiplier = 1 + ((targetStats.maxHealth - targetStats.currentHealth) / targetStats.maxHealth) * 0.833f;
        else if ((targetStats.maxHealth - targetStats.currentHealth) / targetStats.maxHealth > 0.4f) multiplier = 1.5f;
        else multiplier = 2f;
        yield return new WaitForSeconds(0.6f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0], multiplier);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
        float multiplier;
        if ((myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth > 0.6f) multiplier = 1 + ((myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth) * 0.833f;
        else if ((myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth > 0.4f) multiplier = 1.5f;
        else multiplier = 2f;
        yield return new WaitForSeconds(0.6f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0], multiplier);
    }
}