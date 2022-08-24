using Simulator.Combat;
using System.Collections;

public class CheckIfExecutes : Check
{
    private string skill;
    public CheckIfExecutes(ChampionCombat ccombat, string skill) : base(ccombat)
    {
        this.skill = skill;
    }

    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }

    public override bool Control()
    {
        switch (skill)
        {
            case "Q":
                return combat.myStats.qSkill.UseSkill(4, combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            case "W":
                return combat.myStats.wSkill.UseSkill(4, combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            case "E":
                return combat.myStats.eSkill.UseSkill(4, combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            case "R":
                return combat.myStats.rSkill.UseSkill(2, combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            default:
                return false;
        }
    }
}