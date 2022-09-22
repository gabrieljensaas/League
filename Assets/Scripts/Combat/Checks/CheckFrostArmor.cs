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

    public override float Control(float damage)
    {
        if (sejuani.HasFrostArmor)
        {
            sejuani.StartCoroutine(sejuani.FrostArmor());
        }
        return damage;
    }
}