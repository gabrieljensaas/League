using Simulator.Combat;

public class CheckCD : Check
{
    private string skill;
    public CheckCD(ChampionCombat ccombat, string skill) : base(ccombat)
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
                return combat.myStats.qCD <= 0;
            case "W":
                return combat.myStats.wCD <= 0;
            case "E":
                return combat.myStats.eCD <= 0;
            case "R":
                return combat.myStats.rCD <= 0;
            case "A":
                return combat.attackCooldown <= 0;
            default:
                return false;
        }
    }
}