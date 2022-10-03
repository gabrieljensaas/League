using Simulator.Combat;

public class ZiggsAACheck : Check
{
    public ZiggsAACheck(ChampionCombat ccombat) : base(ccombat)
    {

    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.pCD == 0)
        {
            damage.value += Ziggs.ShortFuseDamageByLevel(combat.myStats.level);
            return damage;
        }
        else
        {
            return damage;
        }

    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}
