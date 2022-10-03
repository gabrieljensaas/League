using Simulator.Combat;

public class CheckSpellShield : Check
{
    public CheckSpellShield(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("SpellShield", out Buff buff))
        {
            buff.Kill();
            damage.value = 0;
            return damage;
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}
