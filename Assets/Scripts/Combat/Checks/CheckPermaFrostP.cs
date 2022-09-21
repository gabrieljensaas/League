using Simulator.Combat;

public class CheckPermaFrostP : Check
{
    public CheckPermaFrostP(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("PermaFrostPassive", out Buff value))
        {
            return value.value == 4;
        }
        return false;
    }

    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}