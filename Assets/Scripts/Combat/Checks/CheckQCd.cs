using Simulator.Combat;

public class CheckQCD : Check
{
    public CheckQCD(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.myStats.qCD > 0 ? false : true;
    }

    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}