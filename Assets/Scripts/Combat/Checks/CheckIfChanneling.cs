using Simulator.Combat;

public class CheckIfChanneling : Check
{
    public CheckIfChanneling(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("Channeling");
    }

    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}