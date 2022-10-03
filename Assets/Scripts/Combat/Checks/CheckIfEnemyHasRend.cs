using Simulator.Combat;

public class CheckIfEnemyHasRend : Check
{
    public CheckIfEnemyHasRend(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.targetStats.buffManager.buffs.ContainsKey("Rend");
    }
    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}