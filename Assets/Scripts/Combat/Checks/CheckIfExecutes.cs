using Simulator.Combat;

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

    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }

    public override bool Control()
    {
        switch (skill)
        {
            case "Q":
                return combat.QSkill().UseSkill(combat.myStats.qLevel, combat.qKeys[0], combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            case "W":
                return combat.WSkill().UseSkill(combat.myStats.wLevel, combat.wKeys[0], combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            case "E":
                return combat.ESkill().UseSkill(combat.myStats.eLevel, combat.eKeys[0], combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            case "R":
                return (combat.RSkill().UseSkill(combat.myStats.rLevel, combat.rKeys[0], combat.myStats, combat.targetStats) * multiplier) + stack >= combat.targetStats.currentHealth;
            case "Riven":
                return combat.RSkill(1).UseSkill(combat.myStats.rLevel, combat.rKeys[0], combat.myStats, combat.targetStats) * (1 + ((combat.targetStats.maxHealth - combat.targetStats.currentHealth) / combat.targetStats.maxHealth) > 0.75f ? 2 : (combat.targetStats.maxHealth - combat.targetStats.currentHealth) * 2.667f) >= combat.targetStats.currentHealth;
            case "SylasRiven":
                return combat.RSkill(1).UseSkill(combat.myStats.rLevel, combat.rKeys[0], combat.targetStats, combat.myStats) * (1 + ((combat.myStats.maxHealth - combat.myStats.currentHealth) / combat.myStats.maxHealth) > 0.75f ? 2 : (combat.myStats.maxHealth - combat.myStats.currentHealth) * 2.667f) >= combat.myStats.currentHealth;
            case "Kalista":
                if (combat.TargetBuffManager.buffs.TryGetValue("Rend", out Buff value))
                {
                    return combat.ESkill().UseSkill(combat.myStats.eLevel, combat.eKeys[0], combat.myStats, combat.targetStats) + ((value.value - 1) * combat.ESkill().UseSkill(combat.myStats.eLevel, combat.eKeys[1], combat.myStats, combat.targetStats)) >= combat.targetStats.currentHealth;
                }
                return combat.ESkill().UseSkill(combat.myStats.eLevel, combat.eKeys[0], combat.myStats, combat.targetStats) >= combat.targetStats.currentHealth;
            default:
                return false;
        }
    }
}