using Simulator.Combat;

public class CheckIfEnemyDashedOrBlinked : Check
{
    public CheckIfEnemyDashedOrBlinked(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        if(damage.skillComponentType.HasFlag(SkillComponentTypes.Blink) || damage.skillComponentType.HasFlag(SkillComponentTypes.Dash))
        {
            combat.TargetBuffManager.Add("Gloom" ,new GloomBuff(6, combat.TargetBuffManager));
        }
        return damage;
    }
}