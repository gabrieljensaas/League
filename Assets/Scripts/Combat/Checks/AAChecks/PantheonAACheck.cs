using Simulator.Combat;

public class PantheonAACheck : Check
{
    private Pantheon pantheon;
    public PantheonAACheck(ChampionCombat ccombat) : base(ccombat)
    {

    }

    public override Damage Control(Damage damage)
    {
        if (pantheon.hasMortalWill)
        {
            //need to apply Pantheon W crit
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}
