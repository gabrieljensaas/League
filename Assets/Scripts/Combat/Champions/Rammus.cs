using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Rammus : ChampionCombat
{
    private int durationTimesIncreased;

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

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Magic Damage");

        wKeys.Add("Bonus Armor");
        wKeys.Add("Bonus Magic Resistance");

        eKeys.Add("Taunt Duration");
        eKeys.Add("Bonus Attack Speed Duration");
        eKeys.Add("Bonus Attack Speed");

        rKeys.Add("Center Increased Damage");
        rKeys.Add("Magic Damage per Hit");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if (MyBuffManager.buffs.TryGetValue("AttackSpeedBuff", out Buff value))
        {
            value.duration = ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats);
        }
        MyBuffManager.Add(QSkill().basic.name, new ChannelingBuff(1f, MyBuffManager, QSkill().basic.name, "Channeling"));
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
        TargetBuffManager.Add(QSkill().basic.name, new AirborneBuff(0.1f, TargetBuffManager, QSkill().basic.name));
        TargetBuffManager.Add(QSkill().basic.name, new StunBuff(0.4f, TargetBuffManager, QSkill().basic.name));
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        if (MyBuffManager.buffs.TryGetValue("AttackSpeedBuff", out Buff value))
        {
            value.duration = ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats);
        }
        MyBuffManager.Add(WSkill().basic.name, new DefensiveBallCurlBuff(6, MyBuffManager, WSkill().basic.name));
        MyBuffManager.Add(WSkill().basic.name, new ArmorBuff(6, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), "ArmorBuff"));
        MyBuffManager.Add(WSkill().basic.name, new MagicResistanceBuff(6, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), "MRBuff"));
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        TargetBuffManager.Add("TauntBuff", new TauntBuff(ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), TargetBuffManager, "TauntBuff"));
        MyBuffManager.Add("AttackSpeedBuff", new AttackSpeedBuff(ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), TargetBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), ESkill().basic.name));
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        if (MyBuffManager.buffs.TryGetValue("AttackSpeedBuff", out Buff value))
        {
            value.duration = ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats);
        }
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        TargetBuffManager.Add("KnockOff", new AirborneBuff(0.75f, TargetBuffManager, RSkill().basic.name));
        yield return new WaitForSeconds(1.155f);
        UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats), SkillDamageType.Spell), ESkill().basic.name);
        yield return new WaitForSeconds(1.155f);
        UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats), SkillDamageType.Spell), ESkill().basic.name);
        yield return new WaitForSeconds(1.155f);
        UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats), SkillDamageType.Spell), ESkill().basic.name);

        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (durationTimesIncreased <= 4)
        {
            if (MyBuffManager.buffs.TryGetValue("DefensiveCurl", out Buff value))
            {
                AutoAttack(new Damage(5 + 0.1f * myStats.armor, SkillDamageType.Spell, skillComponentType: SkillComponentTypes.ProcDamage));
                value.duration += 1;
                MyBuffManager.buffs.TryGetValue("ArmorBuff", out Buff value1);
                MyBuffManager.buffs.TryGetValue("MRBuff", out Buff value2);
                value1.duration++;
                value2.duration++;
            }
            durationTimesIncreased++;
        }

        AutoAttack(new Damage(10 + 0.1f * myStats.armor, SkillDamageType.Spell, skillComponentType: SkillComponentTypes.ProcDamage));
    }
}
public class DefensiveBallCurlBuff : Buff
{
    public DefensiveBallCurlBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Defensive Curl For {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Defensive Curl has Ended!");
        manager.buffs.Remove("DefensiveCurl");
    }
}


