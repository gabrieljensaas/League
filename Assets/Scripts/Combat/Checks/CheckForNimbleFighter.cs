using Simulator.Combat;

public class CheckForNimbleFighter : Check
{
    public CheckForNimbleFighter(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        damage.value = damage.value * 0.5f > damage.value - 4 - (combat.myStats.AP * 0.01f) ? damage.value * 0.5f : damage.value - 4 - (combat.myStats.AP * 0.01f);
        return damage;
    }
}