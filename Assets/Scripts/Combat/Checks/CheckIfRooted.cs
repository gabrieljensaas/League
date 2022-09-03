using Simulator.Combat;

public class CheckIfRooted : Check
{
    public CheckIfRooted(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("Root");
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}