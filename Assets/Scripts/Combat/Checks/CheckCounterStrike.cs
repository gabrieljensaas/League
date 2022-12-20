using Simulator.Combat;

public class CheckCounterStrike : Check
{
    public CheckCounterStrike(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.MyBuffManager.buffs.TryGetValue("CounterStrikeBuff", out Buff buff) && damage.skillComponentType == SkillComponentTypes.OnAttack)
        {
            if (buff.value > 5) buff.value++;
            damage.value = 0;
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}