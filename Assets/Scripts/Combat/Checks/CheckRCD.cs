using Simulator.Combat;

public class CheckRCD : Check
{
    public CheckRCD(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.myStats.rCD <= 0;
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}