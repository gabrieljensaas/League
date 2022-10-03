using Simulator.Combat;
using System.Linq;

public class CheckCounterStrike : Check
{
    public CheckCounterStrike(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if(combat.myStats.buffManager.buffs.TryGetValue("CounterStrikeBuff", out Buff buff))
            if (buff.value > 5) buff.value++;

        return 0;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}