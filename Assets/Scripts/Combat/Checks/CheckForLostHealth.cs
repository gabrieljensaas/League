using Simulator.Combat;

public class CheckForLostHealth : Check
{
    private Ekko ekko;
    public CheckForLostHealth(ChampionCombat ccombat, Ekko ekko) : base(ccombat)
    {
        this.ekko = ekko;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        ekko.AddLostHealth(damage.value);
        return damage;
    }
}