using Simulator.Combat;

public class CheckIfFrosted : Check
{
    public CheckIfFrosted(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        return combat.myStats.buffManager.buffs.ContainsKey("Frosted") ? damage * 1.1f : damage;
    }
}