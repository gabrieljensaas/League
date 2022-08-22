using Simulator.Combat;

public class CheckQCDAatrox : Check
{
    public CheckQCDAatrox(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.myStats.qCD > 0 ? false : true;
    }

    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}