using Simulator.Combat;

public class CheckIfUnableToAct : Check
{
    public CheckIfUnableToAct(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("UnableToAct");
    }
    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}