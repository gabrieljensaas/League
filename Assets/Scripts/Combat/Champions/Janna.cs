using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Janna : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "E", "W", "R", "A" };

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
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Maximum Magic Damage");
        
        wKeys.Add("Magic Damage");

        eKeys.Add("Shield Strength");
        eKeys.Add("Bonus Attack Damage");
        
        rKeys.Add("Heal Per Tick");

        if (TargetBuffManager.buffs.ContainsKey("Airborne") || TargetBuffManager.buffs.ContainsKey("Slow") )
        {
            MyBuffManager.Add(ESkill().basic.name, new EyeOfTheStorm(4f, MyBuffManager, ESkill().basic.name));
        }
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)51201);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW) || myStats.wLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)34948);
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        if (MyBuffManager.buffs.ContainsKey("EyeOfTheStorm"))
		{
            MyBuffManager.Add("Shield", new ShieldBuff(5, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats) * 0.15f, "EyeOfTheStorm"));
        }
        MyBuffManager.Add("Shield", new ShieldBuff(5, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), "EyeOfTheStorm"));
        if (MyBuffManager.buffs.ContainsKey("Shield"))
        {
            MyBuffManager.Add("AttackDamage", new AttackDamageBuff(float.MaxValue, MyBuffManager, ESkill().basic.name, (int) ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), "EyeOfTheStorm"));
        }
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        TargetBuffManager.Add("Airborne", new AirborneBuff(0.2f, TargetBuffManager, RSkill().basic.name));
        MyBuffManager.Add("Channeling", new ChannelingBuff(3f, MyBuffManager, RSkill().basic.name, "Monsoon"));
        StartCoroutine(Monsoon());
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public IEnumerator Monsoon()
    {
        if (MyBuffManager.buffs.ContainsKey("EyeOfTheStorm"))
		{
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) *0.15f, RSkill().basic.name); 
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.15f, RSkill().basic.name);
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.15f, RSkill().basic.name);
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.15f, RSkill().basic.name);
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.15f, RSkill().basic.name);
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.15f, RSkill().basic.name);
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.15f, RSkill().basic.name);
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.15f, RSkill().basic.name);
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.15f, RSkill().basic.name);
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.15f, RSkill().basic.name);
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.15f, RSkill().basic.name);
            yield return new WaitForSeconds(0.25f);
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.15f, RSkill().basic.name);
        }
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.25f);
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
    }
}
public class EyeOfTheStorm : Buff
{
    public EyeOfTheStorm(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has Shield & Heal power for {duration} Seconds");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Shield & Heal power");
        manager.buffs.Remove("EyeOfTheStorm");
    }
}