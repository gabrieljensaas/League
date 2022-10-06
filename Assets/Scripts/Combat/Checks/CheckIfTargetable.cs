using Simulator.Combat;

public class CheckIfTargetable : Check
{
    public CheckIfTargetable(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        damage.value = (combat.myStats.buffManager.buffs.ContainsKey("Untargetable") || combat.myStats.buffManager.buffs.ContainsKey("Stasis")) ? 0 : damage.value;
        return damage;
    }
}