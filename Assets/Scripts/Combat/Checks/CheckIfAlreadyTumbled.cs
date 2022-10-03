using Simulator.Combat;

public class CheckIfAlreadyTumbled : Check
{
    public CheckIfAlreadyTumbled(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("Tumble");
    }
    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}