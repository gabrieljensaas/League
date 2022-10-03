using Simulator.Combat;

public class CheckForMundosGreyHealth : Check
{
    private DrMundo drMundo;
    public CheckForMundosGreyHealth(ChampionCombat ccombat, DrMundo drMundo) : base(ccombat)
    {
        this.drMundo = drMundo;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (drMundo.WActive) drMundo.GreyHealth += damage * combat.myStats.wSkill[0].UseSkill(4, combat.wKeys[1], combat.myStats, combat.targetStats);
        return damage;
    }
}