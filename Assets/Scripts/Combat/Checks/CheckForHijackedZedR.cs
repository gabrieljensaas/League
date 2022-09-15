using Simulator.Combat;

public class CheckForHijackedZedR : Check
{
    private Zed zed;
    public CheckForHijackedZedR(ChampionCombat ccombat, Zed zed) : base(ccombat)
    {
        this.zed = zed;
    }

    public override float Control(float damage)
    {
        if (zed.hMarkedForDeath) zed.hMarkedRawDamage += damage;
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}