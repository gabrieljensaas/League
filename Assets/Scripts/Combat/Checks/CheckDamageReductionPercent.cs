using Simulator.Combat;

public class CheckDamageReductionPercent : Check
{
    public CheckDamageReductionPercent(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("DamageReduction", out Buff value))
        {
            damage *= (100 - value.value) / 100;
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}