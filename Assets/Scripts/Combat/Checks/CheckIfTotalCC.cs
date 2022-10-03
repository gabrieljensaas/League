using Simulator.Combat;

public class CheckIfTotalCC : Check
{
    public CheckIfTotalCC(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.HasTotalCC;
    }
    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}