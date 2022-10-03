using Simulator.Combat;

public class SivirAACheck : Check
{
    public SivirAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.buffs.ContainsKey("OnTheHunt"))
        {
            combat.myStats.qCD -= 0.5f;
            combat.myStats.wCD -= 0.5f;
            combat.myStats.eCD -= 0.5f;
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}
