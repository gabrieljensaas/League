using Simulator.Combat;

public class AkaliAACheck : Check
{
    public AkaliAACheck(ChampionCombat ccombat) : base(ccombat)
    {

    }

    public override float Control(float damage)
    {
        return damage + (combat.myStats.baseAD + Akali.AkaliPassiveDamageByLevel[combat.myStats.level] + (combat.myStats.bonusAD * 0.6f) + (0.55f * combat.myStats.AP));
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}
