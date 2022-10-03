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

        autoattackcheck = new DariusAACheck(this);
        dariusP = new CheckDariusP(this);

        qKeys.Add("Blade Physical Damage");
        wKeys.Add("Physical Damage");
        eKeys.Add("Armor Penetration");
        rKeys.Add("True Damage");

        targetStats.armor *= (100 - myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats)) * 0.01f;
        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        CheckDariusPassiveHemorrhage("Auto Attack");
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        UpdateTotalHeal(ref hSum, (myStats.maxHealth - myStats.currentHealth) * 0.13f, myStats.qSkill[0].basic.name);
        CheckDariusPassiveHemorrhage(myStats.qSkill[0].basic.name);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Crippling Strike", new CripplingStrikeBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats)));
        CheckDariusPassiveHemorrhage(myStats.wSkill[0].basic.name);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.1f, targetStats.buffManager, myStats.eSkill[0].basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;
        if (!dariusP.Control()) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, new Damage(myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats) * (1 + (0.2f * (int)targetStats.buffManager.buffs["Hemorrhage"]?.value)), SkillDamageType.True), "Noxian Guillotine");
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    private void CheckDariusPassiveHemorrhage(string skillName)
    {
        if (myStats.buffManager.buffs.TryGetValue("Noxian Might", out Buff noxianMight))
        {
            noxianMight.duration = 5;
            targetStats.buffManager.buffs["Hemorrhage"].duration = 5;
        }
        else if (dariusP.Control())
        {
            myStats.buffManager.buffs.Add("Noxian Might", new AttackDamageBuff(5, myStats.buffManager, "Noxian Might", GetDariusNoxianMightByLevel(myStats.level), "Noxian Might"));
            targetStats.buffManager.buffs["Hemorrhage"].duration = 5;
        }
        else if (targetStats.buffManager.buffs.TryGetValue("Hemorrhage", out Buff hemorrhage))
        {
            hemorrhage.value++;
            hemorrhage.duration = 5;
            simulationManager.ShowText($"{targetStats.name} Gained A Stack of Hemorrhage From {skillName}");
        }
        else
        {
            targetStats.buffManager.buffs.Add("Hemorrhage", new HemorrhageBuff(5, targetStats.buffManager, skillName));
        }
    }
}