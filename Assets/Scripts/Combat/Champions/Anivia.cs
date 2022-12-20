using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Anivia : ChampionCombat
{
    private static float CheckBonusResistanceByLevel(int level)
    {
        return level switch
        {
            < 5 => -40,
            < 8 => -25,
            < 12 => -10,
            < 15 => 5,
            _ => 20,
        };
    }
    public bool isChilled = false;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "E", "W", "A" };

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
        checksR.Add(new CheckIfChanneling(this));
        //checkTakeDamagePostMitigation.Add(new CheckAniviaP(this, this));


        qKeys.Add("Magic Damage");
        qKeys.Add("Magic Damage");
        qKeys.Add("Stun Duration");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage per Tick");
        rKeys.Add("Empowered Damage per Tick");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)18564);
        TargetBuffManager.Add("Stun", new StunBuff(QSkill().UseSkill(myStats.qLevel, qKeys[2], myStats, targetStats), TargetBuffManager, QSkill().basic.name));
        if (TargetBuffManager.buffs.TryGetValue("ChilledBuff", out Buff buff))
            buff.duration = 3;
        else
            TargetBuffManager.Add("Chilled", new ChilledBuff(3, TargetBuffManager, "Chilled"));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)18564, buffNames: new string[] {"Stun", "Chilled"});
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        TargetBuffManager.Add("Airborne", new AirborneBuff(0.1f, TargetBuffManager, WSkill().basic.name));
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        if (TargetBuffManager.buffs.ContainsKey("Chilled"))
            UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], damageModifier: 2, skillComponentTypes: (SkillComponentTypes)34948);
        else UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)34948);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        StartCoroutine(GlacialStorm());
        MyBuffManager.Add("Channeling", new ChannelingBuff(float.MaxValue, MyBuffManager, RSkill().basic.name, "GlacialStorm"));
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Anivia's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public IEnumerator GlacialStorm()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateTotalDamage(ref rSum, 4, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)8192);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalDamage(ref rSum, 4, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)8192);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalDamage(ref rSum, 4, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)8192);
        StartCoroutine(EmpoweredGlacialStorm());
        ((ChannelingBuff)MyBuffManager.buffs["Channeling"]).uniqueKey = "EmpoweredGlacialStorm";

    }

    public IEnumerator EmpoweredGlacialStorm()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateTotalDamage(ref rSum, 4, RSkill(), myStats.rLevel, rKeys[1], skillComponentTypes: (SkillComponentTypes)8192);
        if (TargetBuffManager.buffs.TryGetValue("ChilledBuff", out Buff buff))
            buff.duration = 3;
        else
            TargetBuffManager.Add("Chilled", new ChilledBuff(3, TargetBuffManager, "Chilled"));
        StartCoroutine(EmpoweredGlacialStorm());
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public void PassiveEgg(float currenthp)
    {
        if (currenthp < 0)
        {
            myStats.currentHealth = myStats.maxHealth;
            MyBuffManager.Add("EggPassive", new UnableToActBuff(6, MyBuffManager, "EggPassive"));
            MyBuffManager.Add("BonusArmor", new ArmorBuff(6, MyBuffManager, myStats.passiveSkill.skillName, CheckBonusResistanceByLevel(myStats.level), "BonusArmor"));
            MyBuffManager.Add("BonusMagicResistance", 
                new MagicResistanceBuff(6, MyBuffManager, myStats.passiveSkill.skillName, (int)CheckBonusResistanceByLevel(myStats.level), "BonusMagicResistance"));
        }
    }
}