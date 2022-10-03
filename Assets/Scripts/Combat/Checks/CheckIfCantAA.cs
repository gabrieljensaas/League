using Simulator.Combat;

public class CheckIfCantAA : Check
{
    public CheckIfCantAA(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("CantAA");
    }
    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}