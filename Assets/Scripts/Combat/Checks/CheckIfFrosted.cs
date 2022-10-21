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
    public override Damage Control(Damage damage)
    {
        if(damage.skillComponentType == SkillComponentTypes.OnHit && combat.myStats.buffManager.buffs.ContainsKey("Frosted")) damage.value *= 1.1f;
        return damage;
    }
}