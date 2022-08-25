using Simulator.Combat;
using System.Collections;

public class Darius : ChampionCombat
{
    private CheckDariusP dariusP;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "A", "W", "Q", "R" };

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

        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill.basic.castTime));
        CheckDariusPassiveHemorrhage(myStats.qSkill.basic.name);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill, 4);
        myStats.qCD = myStats.qSkill.basic.coolDown[4];
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
            myStats.buffManager.buffs.Add("Noxian Might", new AttackDamageBuff(5, myStats.buffManager, skillName, Constants.GetDariusNoxianMightByLevel(myStats.level), myStats.passiveSkill.skillName));
            targetStats.buffManager.buffs["Hemorrhage"].duration = 5;
            simulationManager.ShowText($"{targetStats.name} Gained {Constants.GetDariusNoxianMightByLevel(myStats.level)} AD From {skillName}");
        }
        else if (targetStats.buffManager.buffs.TryGetValue("Hemorrhage", out Buff hemorrhage))
        {
            hemorrhage.value++;
            hemorrhage.duration = 5;
            simulationManager.ShowText($"{targetStats.name} Gained A Stack of Hemorrhage From {skillName}");
        }
        else
        {
            targetStats.buffManager.buffs.Add("Hemorrhage", new HemorrhageBuff(5, myStats.buffManager, skillName));
        }
    }
}
