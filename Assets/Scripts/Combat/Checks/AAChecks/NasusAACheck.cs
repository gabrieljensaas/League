using Simulator.Combat;

public class NasusAACheck : Check
{
    public NasusAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("SiphoningStrike", out Buff value))
        {
            damage.value += value.value;
            combat.qSum += value.value;
            combat.myUI.abilitySum[0].text = combat.qSum.ToString();
            combat.myStats.buffManager.buffs.Remove("SiphoningStrike");
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}