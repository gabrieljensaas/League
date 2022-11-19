using Simulator.Combat;
using System.Collections;

public class Darius : ChampionCombat
{
    public static int GetDariusNoxianMightByLevel(int level)
    {
        if (level < 10) return 30 + (5 * (level - 1));
        else if (level < 13) return 75 + (10 * (level - 10));
        else return 105 + (25 * (level - 13));
    }

    public static float GetDariusHemorrhageByLevel(int level, int stack) => (3f * stack) + (0.25f * stack * level);
    public static float GetDariusArmorReductionByLevel(int level) => 10 + (level * 5);
    public static float GetDariusNoxianGuillotineByLevel(int level, int stack)
    {
        if (level < 6) return 125 + (stack * .2f * 125);
        else if (level < 11) return 250 + (stack * .2f * 250);
        else return 375 + (stack * .2f * 375);
    }

    private CheckDariusP dariusP;

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

        autoattackcheck = new DariusAACheck(this);
        dariusP = new CheckDariusP(this);

        qKeys.Add("Blade Physical Damage");
        wKeys.Add("Physical Damage");
        eKeys.Add("Armor Penetration");
        rKeys.Add("True Damage");

        targetStats.armor *= (100 - myStats.eSkill[0].UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats)) * 0.01f;
        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Darius's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
        CheckDariusPassiveHemorrhage("Auto Attack");
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes:(SkillComponentTypes)18560);
        UpdateTotalHeal(ref hSum, (myStats.maxHealth - myStats.currentHealth) * 0.13f, QSkill().basic.name);
        CheckDariusPassiveHemorrhage(QSkill().basic.name);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        MyBuffManager.Add("Crippling Strike", new CripplingStrikeBuff(4, MyBuffManager, WSkill().basic.name,WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats)));
        CheckDariusPassiveHemorrhage(WSkill().basic.name);
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        TargetBuffManager.Add("Airborne", new AirborneBuff(0.1f, TargetBuffManager, ESkill().basic.name));
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;
        if (!dariusP.Control()) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * (1 + (0.2f * (int)TargetBuffManager.buffs["Hemorrhage"]?.value)), SkillDamageType.True,(SkillComponentTypes)34944), "Noxian Guillotine");
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 4);
    }

    private void CheckDariusPassiveHemorrhage(string skillName)
    {
        if (MyBuffManager.buffs.TryGetValue("Noxian Might", out Buff noxianMight))
        {
            noxianMight.duration = 5;
            TargetBuffManager.buffs["Hemorrhage"].duration = 5;
        }
        else if (dariusP.Control())
        {
            MyBuffManager.buffs.Add("Noxian Might", new AttackDamageBuff(5, MyBuffManager, "Noxian Might", GetDariusNoxianMightByLevel(myStats.level), "Noxian Might"));
            TargetBuffManager.buffs["Hemorrhage"].duration = 5;
        }
        else if (TargetBuffManager.buffs.TryGetValue("Hemorrhage", out Buff hemorrhage))
        {
            hemorrhage.value++;
            hemorrhage.duration = 5;
            simulationManager.ShowText($"{targetStats.name} Gained A Stack of Hemorrhage From {skillName}");
        }
        else
        {
            TargetBuffManager.buffs.Add("Hemorrhage", new HemorrhageBuff(5, TargetBuffManager, skillName));
        }
    }
}