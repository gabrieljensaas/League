using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Nasus : ChampionCombat
{
    public int qStack = 0;        // nasus q has unlimited stack size, we will adjust this value by rounding it with game time
    public static float NasusPassiveLifeSteal(int level)
    {
        return level switch
        {
            < 7 => 0.09f,
            < 13 => 0.14f,
            _ => 0.19f
        };
    }

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
        autoattackcheck = new NasusAACheck(this);

        qKeys.Add("Bonus Physical Damage");

        wKeys.Add("Maximum Cripple");
        wKeys.Add("Additional Cripple Per Second");

        eKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage Per Tick");
        eKeys.Add("Armor Reduction");

        rKeys.Add("Bonus Health");
        rKeys.Add("Bonus Resistances");
        rKeys.Add("Magic Damage Per Tick");

        myStats.lifesteal += NasusPassiveLifeSteal(myStats.level);
        base.UpdatePriorityAndChecks();
    }
    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Nasus's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        MyBuffManager.Add("SiphoningStrike", new SiphoningStrikeBuff(10f, MyBuffManager, QSkill().basic.name, QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats) + qStack));
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
        attackCooldown = 0;
    }
    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        TargetBuffManager.Add("CrippleBuff", new CrippleBuff(1, TargetBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats)));
        TargetBuffManager.Add("CrippleBuff", new CrippleBuff(1, TargetBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) + WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats)));
        TargetBuffManager.Add("CrippleBuff", new CrippleBuff(1, TargetBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) + WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats)));
        TargetBuffManager.Add("CrippleBuff", new CrippleBuff(1, TargetBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) + WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats)));
        TargetBuffManager.Add("CrippleBuff", new CrippleBuff(1, TargetBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) + WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats)));
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)10240);
        TargetBuffManager.Add("ArmorReduction", new ArmorReductionBuff(6f, TargetBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[2], myStats, targetStats), "ArmorReduction"));
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
        StartCoroutine(SpiritFire());
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        MyBuffManager.Add("ArmorBuff", new ArmorBuff(rSum, MyBuffManager, RSkill().basic.name, RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats), "ArmorBuff"));
        MyBuffManager.Add("MRBuff", new MagicResistanceBuff(rSum, MyBuffManager, RSkill().basic.name, (int)RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats), "MRBuff"));
        StartCoroutine(FuryOfTheSands(0));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public IEnumerator SpiritFire()
    {
        yield return new WaitForSeconds(0.25f);
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[1], skillComponentTypes:(SkillComponentTypes)8192);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[1], skillComponentTypes: (SkillComponentTypes)8192);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[1], skillComponentTypes: (SkillComponentTypes)8192);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[1], skillComponentTypes: (SkillComponentTypes)8192);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[1], skillComponentTypes: (SkillComponentTypes)8192);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[1], skillComponentTypes: (SkillComponentTypes)8192);
    }

    public IEnumerator FuryOfTheSands(float time)
    {
        if (time == 0)
        {
            myStats.maxHealth += RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats);
            myStats.currentHealth += RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats);
            myStats.bonusHP += RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats);
            QSkill().basic.coolDown[myStats.qLevel] *= 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            UpdateTotalDamage(ref rSum, 2, RSkill(), myStats.rLevel, rKeys[2], skillComponentTypes: (SkillComponentTypes)8192);
        }

        if (time != 15f)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(FuryOfTheSands(time + 0.5f));
        }
        else
        {
            myStats.maxHealth -= RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats);
            myStats.currentHealth -= RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats);
            myStats.bonusHP -= RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats);
            QSkill().basic.coolDown[myStats.qLevel] *= 2f;
        }
    }
}