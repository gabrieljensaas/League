using Simulator.Combat;

public class CheckIfBlockingProjectile : Check
{
    public bool blocking;
    public CheckIfBlockingProjectile(ChampionCombat ccombat, ref bool blocking) : base(ccombat)
    {
        this.blocking = blocking;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        if (damage.skillComponentType.HasFlag(SkillComponentTypes.Projectile))
        {
            damage.value = float.MinValue;
        }
        return damage;
    }
}