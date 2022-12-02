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
        timeSinceE += Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, new Damage(1.2f * myStats.AD, SkillDamageType.Phyiscal, skillComponentType:(SkillComponentTypes)792), QSkill().basic.name);
        UpdateTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats) * targetStats.maxHealth, SkillDamageType.Spell, skillComponentType:(SkillComponentTypes)34944), QSkill().basic.name);
        UpdateTotalHeal(ref qSum, QSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), QSkill().basic.name);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }
    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        MyBuffManager.Add("AttackSpeedBuff", new AttackSpeedBuff(8f, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), "Blood Hunt"));
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
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
            myStats.eCD = ESkill().basic.coolDown[myStats.eLevel] - timeSinceE;
        }
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        MyBuffManager.Add("ImmuneToCC", new ImmuneToCCBuff(0.1f, MyBuffManager, "CC-Immune", "InfiniteDuress"));
        TargetBuffManager.Add("KnockdownBuff", new KnockdownBuff(0.1f, TargetBuffManager, RSkill().basic.name));
        MyBuffManager.Add("Channeling", new ChannelingBuff(1.5f, MyBuffManager, RSkill().basic.name, "InfiniteDuress"));
        TargetBuffManager.Add("SuppressBuff", new SuppressionBuff(1.5f, TargetBuffManager, RSkill().basic.name));

        UpdateTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), SkillDamageType.Spell,skillComponentType:(SkillComponentTypes)35736), RSkill().basic.name);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name); //need to change to post mitigation
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 4);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Warwick's Auto Attack") != float.MinValue)
		{
            UpdateTotalDamage(ref aSum, 5, new Damage(10 + 2 * myStats.level, SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)808), myStats.passiveSkill.name);
            if (myStats.currentHealth < 0.5f * myStats.maxHealth)
            {
                if (myStats.currentHealth > 0.25f * myStats.maxHealth) UpdateTotalHeal(ref pSum, 10 + 2 * myStats.level, myStats.passiveSkill.skillName); //need to fix post mitigation damage
                else UpdateTotalHeal(ref pSum, 2.5f * (10 + 2 * myStats.level), myStats.passiveSkill.skillName);
            }
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }
}