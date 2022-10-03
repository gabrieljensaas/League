using Simulator.Combat;

public class ViegoAACheck : Check
{
    public ViegoAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {



        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}