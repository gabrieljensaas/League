using Simulator.Combat;

public class AkaliAACheck : Check
{
    public AkaliAACheck(ChampionCombat ccombat) : base(ccombat)
    {

    }

    public override Damage Control(Damage damage)
    {
        //damage.value += (combat.myStats.baseAD + Akali.AkaliPassiveDamageByLevel[combat.myStats.level] + (combat.myStats.bonusAD * 0.6f) + (0.55f * combat.myStats.AP));
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}
