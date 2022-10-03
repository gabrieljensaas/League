using Simulator.Combat;

public class ZoeAACheck : Check
{
    private Zoe zoe;
    public ZoeAACheck(ChampionCombat ccombat, Zoe zoe) : base(ccombat)
    {
        this.zoe = zoe;
    }

    public override Damage Control(Damage damage)
    {
        if (zoe.HasPassive)
        {
            damage.value += Zoe.ZoePassiveDamageByLevel[combat.myStats.level] + (combat.myStats.AP * 0.2f);
            zoe.HasPassive = false;
            zoe.StopCoroutine(zoe.ZoePassive());
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}