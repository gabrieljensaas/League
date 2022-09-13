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
    public override float Control(float damage)
    {
        return combat.myStats.buffManager.buffs.ContainsKey("Hemoplague") ? damage * 1.1f : damage;
    }
}