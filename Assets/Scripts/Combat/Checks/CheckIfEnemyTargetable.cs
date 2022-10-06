using Simulator.Combat;

public class CheckIfEnemyTargetable : Check
{
    public CheckIfEnemyTargetable(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !(combat.targetStats.buffManager.buffs.ContainsKey("Untargetable") || combat.targetStats.buffManager.buffs.ContainsKey("Stasis"));
    }
    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}