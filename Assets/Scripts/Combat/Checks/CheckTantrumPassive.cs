using Simulator.Combat;

public class CheckTantrumPassive : Check
{
    private Amumu amumu;
    public CheckTantrumPassive(ChampionCombat ccombat, Amumu amumu) : base(ccombat)
    {
        this.amumu = amumu;
    }

    public override Damage Control(Damage damage)
    {
        amumu.DamagedByAutoAttack(damage.value);
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}