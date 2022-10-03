using Simulator.Combat;

public class CheckIfChanneling : Check
{
    public CheckIfChanneling(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("Channeling");
    }

    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}