using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Warwick : ChampionCombat
{
	private bool eCast;
    private float timeSinceE;
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
        checksQ.Add(new CheckIfImmobilize(this));
        checksR.Add(new CheckIfImmobilize(this));

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Bonus Magic Damage");
        qKeys.Add("Healing Percentage");
        wKeys.Add("Bonus Attack Speed");
        wKeys.Add("Increased Attack Speed");
        eKeys.Add("Damage Reduction");
        rKeys.Add("Total Magic Damage");

        if (targetStats.currentHealth < 0.5f * targetStats.maxHealth)
        {
            if (targetStats.currentHealth > 0.25f * targetStats.maxHealth) MyBuffManager.Add("AttackSpeedBuff", new AttackSpeedBuff(float.MaxValue, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), "BloodHunt"));
            else MyBuffManager.Add("AttackSpeedBuff", new AttackSpeedBuff(float.MaxValue, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), "BloodHunt"));
        }

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceE += Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, new Damage(1.2f * myStats.AD, SkillDamageType.Phyiscal), QSkill().basic.name);
        UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats) * targetStats.maxHealth, SkillDamageType.Spell), QSkill().basic.name); 
        UpdateTotalHeal(ref qSum, QSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), QSkill().basic.name);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        if (!eCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            MyBuffManager.Add("DamageReductionBuff", new DamageReductionPercentBuff(2.5f, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats)));
            myStats.eCD = 1f;
            timeSinceE = 0;
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            MyBuffManager.Add("CantAABuff", new CantAABuff(0.5f, MyBuffManager, "CantAA"));
            yield return new WaitForSeconds(0.5f);
            TargetBuffManager.Add("FearBuff", new FleeBuff(1f, TargetBuffManager, ESkill().basic.name));
            eCast = false;
            myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel] - timeSinceE;
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        MyBuffManager.Add("ImmuneToCC", new ImmuneToCCBuff(0.1f, MyBuffManager, "CC-Immune", "InfiniteDuress"));
        TargetBuffManager.Add("KnockdownBuff", new KnockdownBuff(0.1f, TargetBuffManager, RSkill().basic.name));
        MyBuffManager.Add("Channeling", new ChannelingBuff(1.5f, MyBuffManager, RSkill().basic.name, "InfiniteDuress"));
        TargetBuffManager.Add("SuppressBuff", new SuppressionBuff(1.5f, TargetBuffManager, RSkill().basic.name));

        UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Blockable | SkillComponentTypes.Dash |SkillComponentTypes.OnHit), QSkill().basic.name);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name); //need to change to post mitigation
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if (myStats.currentHealth < 0.5f * myStats.maxHealth)
        {
            if (myStats.currentHealth > 0.25f * myStats.maxHealth) UpdateTotalHeal(ref pSum, 10 + 2 * myStats.level, myStats.passiveSkill.skillName); //need to fix post mitigation damage
            else UpdateTotalHeal(ref pSum, 2.5f * (10 + 2 * myStats.level), myStats.passiveSkill.skillName);
        }
        AutoAttack(new Damage(10 + 2 * myStats.level, SkillDamageType.Spell, SkillComponentTypes.ProcDamage | SkillComponentTypes.OnHit));
    }
}