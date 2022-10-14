using Simulator.Combat;

public class CheckGalioWP : Check
{
    private Galio galio;
    public CheckGalioWP(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        if (galio.hasShieldOfDurandPassive) galio.hasShieldOfDurandPassive = false;
        return damage;
    }
}