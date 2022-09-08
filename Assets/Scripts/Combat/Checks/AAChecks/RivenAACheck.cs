using Simulator.Combat;

public class RivenAACheck : Check
{
    public RivenAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("RunicBlade", out Buff value))
        {
            value.value--;
            value.duration = 6;
            damage += Constants.GetRivenPassiveDamagePercentByLevel(combat.myStats.level) * combat.myStats.AD;
            if (value.value == 0) value.Kill();
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}