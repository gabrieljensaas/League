using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Yuumi : ChampionCombat
{
    private static float[] PassiveShieldByLevel = { 60, 70, 80, 95, 110, 125, 140, 160, 180, 200, 240, 260, 280, 305, 330, 355, 380 };

    private float pCD;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "E", "R", "A", "W" };

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
        checkTakeDamagePostMitigation.Add(new CheckShield(this));
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Magic Damage");

        eKeys.Add("Heal");
        eKeys.Add("Bonus Attack Speed");

        rKeys.Add("Magic Damage Per Wave");
        rKeys.Add("Reduced Damage Per Wave");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        pCD -= Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable | SkillComponentTypes.Blockable);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateTotalHeal(ref eSum, ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), ESkill().basic.name);
        MyBuffManager.Add("AttackSpeed", new AttackSpeedBuff(3f, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), "Zoomies"));
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes:SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        yield return new WaitForSeconds(0.5f);
        TargetBuffManager.Add("Root", new RootBuff(1.75f, TargetBuffManager, RSkill().basic.name));
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        yield return new WaitForSeconds(0.5f);
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));

        if (AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal)).damage != float.MinValue && pCD <= 0)
        {
            MyBuffManager.Add("Shield", new ShieldBuff(float.MaxValue, MyBuffManager, myStats.passiveSkill.skillName, PassiveShieldByLevel[myStats.level], "BopN'Block"));
            pCD = 14 - 8 / 17 * (myStats.level - 1);
        }
    }
}