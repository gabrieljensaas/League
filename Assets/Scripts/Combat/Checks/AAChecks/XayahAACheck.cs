using Simulator.Combat;

public class XayahAACheck : Check
{
    private Xayah xayah;
    public XayahAACheck(ChampionCombat ccombat, Xayah xayah) : base(ccombat)
    {
        this.xayah = xayah;
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.buffs.ContainsKey(combat.myStats.wSkill[0].basic.name)) damage.value *= 1.2f;
        if (xayah.feathersAtHand > 0)
        {
            damage.value += Xayah.GetXayahPassiveADPercent(combat.myStats.level) * 0.01f * combat.myStats.AD;
            xayah.StartCoroutine(xayah.FeatherInGround());
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}