using Simulator.Combat;

public class CheckIfEnemyAirborne : Check
{
    public CheckIfEnemyAirborne(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.targetStats.buffManager.buffs.ContainsKey("Airborne");
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}