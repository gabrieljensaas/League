using Simulator.Combat;

public class CheckIfAirborne : Check
{
    public CheckIfAirborne(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("Airborne");
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}