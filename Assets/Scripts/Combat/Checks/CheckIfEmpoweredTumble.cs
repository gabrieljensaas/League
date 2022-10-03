using Simulator.Combat;

public class CheckIfEmpoweredTumble : Check
{
    public CheckIfEmpoweredTumble(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return combat.myStats.buffManager.buffs.ContainsKey("EmpoweredTumble");
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        throw new System.NotImplementedException();
    }
}