using Simulator.Combat;

public class CheckMordekaiserIndestructible : Check
{
    private Mordekaiser mordekaiser;
    public CheckMordekaiserIndestructible(ChampionCombat ccombat, Mordekaiser mordekaiser) : base(ccombat)
    {
        this.mordekaiser = mordekaiser;
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        mordekaiser.Indestructible(damage * 0.15f);
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}