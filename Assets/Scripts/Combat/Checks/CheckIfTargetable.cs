using Simulator.Combat;

public class CheckIfTargetable : Check
{
    public CheckIfTargetable(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        return combat.myStats.buffManager.buffs.ContainsKey("Untargetable") ? 0 : damage;
    }
}