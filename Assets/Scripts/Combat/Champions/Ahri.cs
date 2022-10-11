using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Ahri : ChampionCombat
{
    private int rStacks = 0;
    private int hRStacks = 0;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

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
        checksR.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Damage Per Pass");
        wKeys.Add("Magic Damage");
        wKeys.Add("Additional Magic Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("Disable Duration");
        rKeys.Add("Magic damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == 0) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];

        yield return new WaitForSeconds(2f);                // orb return time estimated by playing Ahri
        UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.True, SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable), QSkill().basic.name);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == 0) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == 0) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Charm", new CharmBuff(myStats.eSkill[0].UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), myStats.buffManager, myStats.eSkill[0].basic.name));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], buffNames: new string[] { "Charm" }, skillComponentTypes: SkillComponentTypes.Projectile |SkillComponentTypes.Spellblockable);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == 0) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        if (rStacks == 0) rStacks = 2;
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        myStats.rCD = rStacks > 0 ? 1 : myStats.rSkill[0].basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal,SkillComponentTypes.Projectile | SkillComponentTypes.OnHit | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable));
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        if (targetStats.buffManager.HasImmobilize) yield break;

        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        if (hRStacks == 0) hRStacks = 2;
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[skillLevel], 2, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        targetStats.rCD = hRStacks > 0 ? 1 : myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
    }
}