using Simulator.Combat;

public class CheckForGnarRage : Check
{
    private Gnar gnar;
    public CheckForGnarRage(ChampionCombat ccombat, Gnar gnar) : base(ccombat)
    {
        this.gnar = gnar;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        gnar.StopCoroutine(gnar.GenerateRage());
        gnar.StartCoroutine(gnar.GenerateRage());
        return damage;
    }
}