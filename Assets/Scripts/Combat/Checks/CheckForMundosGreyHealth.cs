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
    public override Damage Control(Damage damage)
    {
        if (drMundo.WActive) drMundo.GreyHealth += damage.value * combat.myStats.wSkill[0].UseSkill(4, combat.wKeys[1], combat.myStats, combat.targetStats);
        return damage;
    }
}