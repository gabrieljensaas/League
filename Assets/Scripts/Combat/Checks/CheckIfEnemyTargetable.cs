using Simulator.Combat;

public class CheckIfEnemyTargetable : Check
{
    public CheckIfEnemyTargetable(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.targetStats.buffManager.buffs.ContainsKey("Untargetable");
    }
    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}