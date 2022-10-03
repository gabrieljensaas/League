using Simulator.Combat;

public class CheckFrostArmor : Check
{
    private Sejuani sejuani;
    public CheckFrostArmor(ChampionCombat ccombat, Sejuani sejuani) : base(ccombat)
    {
        this.sejuani = sejuani;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }

    public override Damage Control(Damage damage)
    {
        if (sejuani.HasFrostArmor)
        {
            sejuani.StartCoroutine(sejuani.FrostArmor());
        }
        sejuani.passiveTimer = 0;
        return damage;
    }
}