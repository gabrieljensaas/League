using Simulator.Combat;

public class CheckWCD : Check
{
    public CheckWCD(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.myStats.wCD <= 0;
    }

    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}