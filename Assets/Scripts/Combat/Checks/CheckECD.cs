using Simulator.Combat;

public class CheckECD : Check
{
    public CheckECD(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.myStats.eCD > 0 ? false : true;
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}