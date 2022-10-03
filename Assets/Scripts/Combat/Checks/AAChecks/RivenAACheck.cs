using Simulator.Combat;

public class RivenAACheck : Check
{
    public RivenAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("RunicBlade", out Buff value))
        {
            value.value--;
            value.duration = 6;
            damage.value += Riven.GetRivenPassiveDamagePercentByLevel(combat.myStats.level) * combat.myStats.AD;
            if (value.value == 0) value.Kill();
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}