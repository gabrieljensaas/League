using Simulator.Combat;

public class CheckTalonP : Check
{
    public CheckTalonP(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("BladeEnd", out Buff value))
        {
            return value.value == 3;
        }
        return false;
    }

    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}