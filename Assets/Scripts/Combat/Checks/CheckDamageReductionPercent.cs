using Simulator.Combat;

public class CheckDamageReductionPercent : Check
{
    public CheckDamageReductionPercent(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("DamageReduction", out Buff value))
        {
            damage.value *= (100 - value.value) * 0.01f;
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}