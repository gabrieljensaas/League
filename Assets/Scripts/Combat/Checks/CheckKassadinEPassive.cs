using Simulator.Combat;

public class CheckKassadinEPassive : Check
{
    private Kassadin kassadin;
    public CheckKassadinEPassive(ChampionCombat ccombat, Kassadin kassadin) : base(ccombat)
    {
        this.kassadin = kassadin;
    }

    public override bool Control()
    {
        if (combat.targetStats.eLevel > 0) kassadin.eStack++;
        return true;
    }
    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}