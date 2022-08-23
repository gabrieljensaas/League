using Simulator.Combat;

public class CheckACD : Check
{
    public CheckACD(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.attackCooldown <= 0;
    }

    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}