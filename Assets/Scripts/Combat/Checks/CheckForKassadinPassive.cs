using Simulator.Combat;

public class CheckForKassadinPassive : Check
{
    public CheckForKassadinPassive(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        if (damage.damageType == SkillDamageType.Spell) damage.value *= 0.9f;
        return damage;
    }
}