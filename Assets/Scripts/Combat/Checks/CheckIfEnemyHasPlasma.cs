using Simulator.Combat;

public class CheckIfEnemyHasPlasma : Check
{
    public CheckIfEnemyHasPlasma(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.targetStats.buffManager.buffs.ContainsKey("Plasma");
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}