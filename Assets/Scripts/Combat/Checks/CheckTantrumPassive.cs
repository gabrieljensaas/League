using Simulator.Combat;

public class CheckTantrumPassive : Check
{
    private Amumu amumu;
    public CheckTantrumPassive(ChampionCombat ccombat, Amumu amumu) : base(ccombat)
    {
        this.amumu = amumu;
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        amumu.DamagedByAutoAttack(damage);
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}