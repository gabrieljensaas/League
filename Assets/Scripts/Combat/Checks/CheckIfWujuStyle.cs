using Simulator.Combat;

public class CheckIfWujuStyle : Check
{
    public CheckIfWujuStyle(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("WujuStyle");
    }
    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}