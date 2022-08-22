using Simulator.Combat;

public class CheckIfCasting : Check
{
    public CheckIfCasting(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.isCasting;
    }

    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}