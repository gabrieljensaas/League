using Simulator.Combat;

public class CheckIfDisrupt : Check
{
    public CheckIfDisrupt(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.HasDisrupt;
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}