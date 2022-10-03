using Simulator.Combat;

public class ViegoAACheck : Check
{
    public ViegoAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        


        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}