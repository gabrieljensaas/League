using Simulator.Combat;

public class CheckForYoneE : Check
{
    private Yone yone;
    public CheckForYoneE(ChampionCombat ccombat, Yone yone) : base(ccombat)
    {
        this.yone = yone;
    }

    public override Damage Control(Damage damage)
    {
        if (yone.InE && yone.myStats.eLevel > -1) yone.EDamage += damage.value * yone.myStats.eSkill[0].UseSkill(yone.myStats.eLevel, yone.eKeys[0], yone.myStats, combat.myStats);
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}