using Simulator.Combat;

public class CheckIfEnemyAirborne : Check
{
    public CheckIfEnemyAirborne(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.targetStats.buffManager.buffs.ContainsKey("Airborne");
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        throw new System.NotImplementedException();
    }
}