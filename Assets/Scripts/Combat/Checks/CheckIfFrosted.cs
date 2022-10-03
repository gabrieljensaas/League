using Simulator.Combat;

public class CheckIfFrosted : Check
{
    public CheckIfFrosted(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        damage.value *= combat.myStats.buffManager.buffs.ContainsKey("Frosted") ? 1.1f : 1f;
        return damage;
    }
}