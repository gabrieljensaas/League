using Simulator.Combat;

public class CheckIfImmobilize : Check
{
    public CheckIfImmobilize(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.HasImmobilize;
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        throw new System.NotImplementedException();
    }
}