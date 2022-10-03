using Simulator.Combat;

public class CheckIfEnemyHasRend : Check
{
    public CheckIfEnemyHasRend(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.targetStats.buffManager.buffs.ContainsKey("Rend");
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        throw new System.NotImplementedException();
    }
}