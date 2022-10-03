using Simulator.Combat;

public class CheckIfHemoplague : Check
{
    public CheckIfHemoplague(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        damage.value *= combat.myStats.buffManager.buffs.ContainsKey("Hemoplague") ? 1.1f : 1;
        return damage;
    }
}