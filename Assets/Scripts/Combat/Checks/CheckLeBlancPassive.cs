using Simulator.Combat;

public class CheckLeBlancPassive : Check
{
    private LeBlanc lb;
    public CheckLeBlancPassive(ChampionCombat ccombat, LeBlanc lb) : base(ccombat)
    {
        this.lb = lb;
    }

    public override float Control(float damage)
    {
        if (combat.myStats.currentHealth - damage <= combat.myStats.maxHealth * 0.4f && !lb.UsedMirrorImage) lb.MirrorImage();
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}