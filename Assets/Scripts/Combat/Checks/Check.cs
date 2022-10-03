using Simulator.Combat;

public abstract class Check
{
    protected ChampionCombat combat;
    protected Check(ChampionCombat ccombat)
    {
        combat = ccombat;
    }

    public abstract bool Control();
    public abstract float Control(float damage,SkillDamageType damageType, SkillComponentTypes skillComponenetType = SkillComponentTypes.None);
}