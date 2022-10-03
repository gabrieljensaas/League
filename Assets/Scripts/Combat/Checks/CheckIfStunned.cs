using Simulator.Combat;

public class CheckIfStunned : Check
{
    public CheckIfStunned(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("Stun");
    }
    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}