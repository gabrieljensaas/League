using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Leona : ChampionCombat
{
    private float timeSinceShieldIlluminated;
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
        checksA.Add(new CheckIfDisarmed(this));
        checksE.Add(new CheckIfImmobilize(this));
        
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Bonus Magic Damage");

        wKeys.Add("Flat Damage Reduction");
        wKeys.Add("Bonus Armor");
        wKeys.Add("Bonus Magic Resistance");
        wKeys.Add("Magic Damage");
        
        eKeys.Add("Magic Damage");

        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceShieldIlluminated += Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        timeSinceShieldIlluminated = 0;
        yield return new WaitForSeconds(6f);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override  IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW) || myStats.wLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        //Add flat damage reduction
        MyBuffManager.Add("BonusArmor", new ArmorBuff(3f, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), "Eclipse"));
        MyBuffManager.Add("BonusMR", new MagicResistanceBuff(3f, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[2], myStats, targetStats), "Eclipse"));
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[3], skillComponentTypes: SkillComponentTypes.Spellblockable);
        MyBuffManager.Add("BonusArmor", new ArmorBuff(3f, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), "Eclipse"));
        MyBuffManager.Add("BonusMR", new MagicResistanceBuff(3f, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[2], myStats, targetStats), "Eclipse"));
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        TargetBuffManager.Add("Root", new RootBuff(0.5f, TargetBuffManager, ESkill().basic.name));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes:SkillComponentTypes.Dash | SkillComponentTypes.Spellblockable, buffNames: new string[] { "Root" });
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        yield return new WaitForSeconds(3.5f);
        TargetBuffManager.Add("Stun", new StunBuff(1.75f, TargetBuffManager, RSkill().basic.name));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable, buffNames: new string[] { "Stun" });
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if (timeSinceShieldIlluminated < 6 && AutoAttack(new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.ProcDamage | SkillComponentTypes.Spellblockable)).damage != float.MinValue)
		{
            TargetBuffManager.Add("Stun", new StunBuff(1f, TargetBuffManager, QSkill().basic.name));
        }
    }
}