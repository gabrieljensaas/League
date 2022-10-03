using Simulator.Combat;

public class CheckIfEnemyTargetable : Check
{
    public CheckIfEnemyTargetable(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.targetStats.buffManager.buffs.ContainsKey("Untargetable");
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        throw new System.NotImplementedException();
    }
}