using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Gragas : ChampionCombat
{
    private float timeSincePassive = 8f;
    public bool hasDrunkenRage = false;
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

        qKeys.Add("Maximum Magic Damage");

        wKeys.Add("Damage Reduction");
        wKeys.Add("Bonus Magic Damage");

        eKeys.Add("Magic Damage");

        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSincePassive += Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        yield return new WaitForSeconds(4f);
        UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.Spell), QSkill().basic.name);
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        MyBuffManager.Add(WSkill().basic.name, new ChannelingBuff(0.75f, MyBuffManager, WSkill().basic.name, "Channeling"));
        yield return new WaitForSeconds(0.75f);
        MyBuffManager.Add(WSkill().basic.name, new DamageReductionPercentBuff(2.5f, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats)));
        MyBuffManager.Add(WSkill().basic.name, new DrunkenRageBuff(5f, MyBuffManager, WSkill().basic.name));
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Dash), ESkill().basic.name);
        TargetBuffManager.Add("KnockOff", new AirborneBuff(0.1f, TargetBuffManager, ESkill().basic.name));
        TargetBuffManager.Add("StunBuff", new StunBuff(1f, TargetBuffManager, ESkill().basic.name));
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        yield return new WaitForSeconds(0.55f);
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        TargetBuffManager.Add("KnockOff", new AirborneBuff(0.1f, TargetBuffManager, ESkill().basic.name));
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if(hasDrunkenRage)
		{
            AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), SkillDamageType.Spell));
        }
    }

    public void CheckIfPassiveReady()
	{
        if(timeSincePassive >= 8 && myStats.currentHealth < myStats.maxHealth)
		{
            UpdateTotalHeal(ref pSum, 0.65f * myStats.maxHealth, myStats.passiveSkill.skillName);
            timeSincePassive = 0;
        }
	}
}

public class DrunkenRageBuff : Buff
{
    private readonly Gragas gragas;
    public DrunkenRageBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Drunken Rage For {duration} Seconds!");
        gragas.hasDrunkenRage = true;
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Drunken Rage Ended!");
        gragas.hasDrunkenRage = false;
        manager.buffs.Remove("DrunkenRage");
    }
}