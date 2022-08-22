using Simulator.Combat;

public class CheckACD : Check
{
    public CheckACD(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.AttackCooldown > 0 ? false : true;
    }

    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}