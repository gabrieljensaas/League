using Simulator.Combat;
using System.Collections;
using System.Runtime.CompilerServices;

public class CheckIfExecutes : Check
{
    private string skill;
    private int stack;
    private float multiplier;
    public CheckIfExecutes(ChampionCombat ccombat, string skill, int stack = 0, float multiplier = 1) : base(ccombat)
    {
        this.skill = skill;
        this.stack = stack;
        this.multiplier = multiplier;
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
                return combat.myStats.qSkill[0].UseSkill(4, combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            case "W":
                return combat.myStats.wSkill[0].UseSkill(4, combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            case "E":
                return combat.myStats.eSkill[0].UseSkill(4, combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            case "R":
                return (combat.myStats.rSkill[0].UseSkill(2, combat.myStats, combat.targetStats) * multiplier) + stack >= combat.targetStats.currentHealth;
            case "R1":
                return combat.myStats.rSkill[1].UseSkill(2, combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            default:
                return false;
        }
    }
}