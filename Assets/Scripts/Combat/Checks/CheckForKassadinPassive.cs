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
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (damageType == SkillDamageType.Spell) damage *= 0.9f;
        return damage;
    }
}