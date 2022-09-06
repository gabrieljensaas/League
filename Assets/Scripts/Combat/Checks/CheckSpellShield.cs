using Simulator.Combat;
using System.Collections.Generic;

public class CheckSpellShield : Check
{
    public CheckSpellShield(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("SpellShield", out Buff buff))
        {
            buff.Kill();
            return 0;
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}
