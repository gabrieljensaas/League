using Simulator.Combat;

public class CheckIfDisarmed : Check
{
    public CheckIfDisarmed(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("Disarm");
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}