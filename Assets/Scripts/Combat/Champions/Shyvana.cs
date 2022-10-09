using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Shyvana : ChampionCombat
{
    private bool hasTwinBite;
    private bool hasDragonForm;
    private int durationTimesIncreased;
    private bool isMarked;
    private int fury = 100;
    private float timeSinceMarked;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "Q", "A" };

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
        checksR.Add(new CheckIfImmobilize(this));

        qKeys.Add("Bonus Physical Damage");
        wKeys.Add("Magic Damage Per Tick");
        wKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("Increased Damage");
        rKeys.Add("Fury Generation per Second");
        rKeys.Add("Magic Damage");
        rKeys.Add("Bonus Health");

        MyBuffManager.Add("ArmorBuff", new ArmorBuff(float.MaxValue, MyBuffManager, myStats.passiveSkill.skillName, 5, "ArmorBuff"));
        MyBuffManager.Add("MRBuff", new MagicResistanceBuff(float.MaxValue, MyBuffManager, myStats.passiveSkill.skillName, 5, "MRBuff"));

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceMarked += Time.deltaTime;
        fury = Mathf.Clamp(fury, 0, 100);

        if (hasDragonForm)
            fury -= (int)(2.5f * Time.deltaTime * 0.5f);
        if (fury <= 0)
            hasDragonForm = false;
        else
            fury += (int)(RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * Time.deltaTime); 
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        hasTwinBite = true;
        myStats.qCD = QSkill().basic.coolDown[4];
        attackCooldown = 0;
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        MyBuffManager.Add("Burnout", new BurnoutBuff(3f, MyBuffManager, WSkill().basic.name));
        myStats.wCD = WSkill().basic.coolDown[4];
        StartCoroutine(BurnOut());
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        if (!hasDragonForm)
        {
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Projectile), ESkill().basic.name);
        }
        else
        {

            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Projectile), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
        }
        isMarked = true;
        timeSinceMarked = 0;
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;
        if (fury < 100) yield break;
        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[1]);
        hasDragonForm = true;
        MyBuffManager.Add("DragonDescent", new DragonDescentBuff(fury, MyBuffManager, RSkill().basic.name, (int)RSkill().UseSkill(myStats.rLevel, rKeys[2], myStats, targetStats)));
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (hasTwinBite)
        {
            AutoAttack(new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.Phyiscal, SkillComponentTypes.OnHit));
            hasTwinBite = false;
        }
        if (durationTimesIncreased <= 4)
        {
            if (MyBuffManager.buffs.TryGetValue("Burnout", out Buff value))
            {
                AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), SkillDamageType.Spell));
                value.duration += 1;
            }
            durationTimesIncreased++;
        }
        if (isMarked)
        {
            AutoAttack(new Damage(0.35f * targetStats.maxHealth, SkillDamageType.Spell, SkillComponentTypes.OnHit));
            if (timeSinceMarked >=5)
			{
                isMarked = false;
			}   
        }
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        fury += 2;
        myStats.qCD -= 0.5f;
    }

    public IEnumerator BurnOut()
    {
        while (MyBuffManager.buffs["Burnout"].duration >= 0.5f && durationTimesIncreased <= 4)
        {
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        }
    }
}

public class BurnoutBuff : Buff
{
    public BurnoutBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has BurnOut For {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s BurnOut Ended!");
        manager.buffs.Remove("Burnout");
    }
}

public class DragonDescentBuff : Buff
{
    public DragonDescentBuff(int fury,BuffManager manager, string source, int HP) : base(manager)
    {
        manager.stats.maxHealth += HP;
        manager.stats.currentHealth += HP;
        manager.stats.bonusHP += HP;
        value = HP;
        base.source = source;
        base.duration = fury;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {HP} Health from {source}! ");
    }

    public override void Update()
    {
        if (!paused) duration -= 2.5f * Time.deltaTime * 0.5f;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value} Extra Health From {source}!");
        manager.stats.maxHealth -= value;
        manager.stats.currentHealth -= value;
        manager.stats.bonusHP -= value;
        manager.buffs.Remove("DragonDescentBuff");
    }
}