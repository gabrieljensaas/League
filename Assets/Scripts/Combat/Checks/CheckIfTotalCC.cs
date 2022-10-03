using Simulator.Combat;

public class CheckIfTotalCC : Check
{
    public CheckIfTotalCC(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.HasTotalCC;
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        throw new System.NotImplementedException();
    }
}