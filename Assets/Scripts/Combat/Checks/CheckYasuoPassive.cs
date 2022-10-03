using Simulator.Combat;

public class CheckYasuoPassive : Check
{
    private Yasuo yasuo;
    public CheckYasuoPassive(ChampionCombat ccombat, Yasuo yasuo) : base(ccombat)
    {
        this.yasuo = yasuo;
    }

    public override Damage Control(Damage damage)
    {
        if (yasuo.pCD <= 0)
        {
            combat.myStats.buffManager.shields.Add("Way of the Wanderer", new ShieldBuff(1, combat.myStats.buffManager, "Way of the Wanderer", Yasuo.PassiveShieldByLevel[combat.myStats.level], "Way of the Wanderer"));
            yasuo.pCD = Yasuo.pMaxCooldown;
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}