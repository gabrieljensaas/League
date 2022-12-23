using Simulator.Combat;

public class CheckIfBlind : Check
{
    public CheckIfBlind(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (damage.skillComponentType == SkillComponentTypes.Blindable) damage.value = float.MinValue;
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}