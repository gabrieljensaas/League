using Simulator.Combat;
using System.Collections;

public class Darius : ChampionCombat
{
    private CheckDariusP dariusP;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "W", "A", "R", "" };

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

        targetCombat.checksQ.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksW.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksE.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksR.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksA.Add(new CheckIfStunned(targetCombat));

        autoattackcheck = new DariusAACheck(this);
        dariusP = new CheckDariusP(this);
        targetStats.armor *= (100 - Constants.GetDariusArmorReductionByLevel(5)) / 100;

        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
        CheckDariusPassiveHemorrhage("Auto Attack");
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill.basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill, 4);
        UpdateTotalHeal(ref hSum, (myStats.maxHealth - myStats.currentHealth) * 0.13f , myStats.qSkill.basic.name);
        CheckDariusPassiveHemorrhage(myStats.qSkill.basic.name);
        myStats.qCD = myStats.qSkill.basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill.basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill, 4);
        CheckDariusPassiveHemorrhage(myStats.wSkill.basic.name);
        myStats.wCD = myStats.wSkill.basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill.basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, Constants.GetDariusNoxianGuillotineByLevel(myStats.level, (int)targetStats.buffManager.buffs["Hemorrhage"]?.value), "Noxian Guillotine", SkillList.SkillDamageType.True);
        myStats.rCD = myStats.rSkill.basic.coolDown[2];
    }

    private void CheckDariusPassiveHemorrhage(string skillName)
    {
        if(myStats.buffManager.buffs.TryGetValue("Noxian Might", out Buff noxianMight))
        {
            noxianMight.duration = 5;
            targetStats.buffManager.buffs["Hemorrhage"].duration = 5;
        }
        else if (dariusP.Control())
        {
            myStats.buffManager.buffs.Add("Noxian Might", new AttackDamageBuff(5, myStats.buffManager, "Noxian Might", Constants.GetDariusNoxianMightByLevel(myStats.level), "Noxian Might"));
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
