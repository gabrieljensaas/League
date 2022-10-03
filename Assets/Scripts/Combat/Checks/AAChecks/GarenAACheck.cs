using Simulator.Combat;

public class GarenAACheck : Check
{
    public GarenAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("DecisiveStrike", out Buff value))
        {
            damage.value += value.value;
            combat.qSum += value.value;
            combat.myUI.abilitySum[0].text = combat.qSum.ToString();
            combat.myStats.buffManager.buffs.Remove("DecisiveStrike");
            combat.targetStats.buffManager.buffs.Add("Silence", new SilenceBuff(1.5f, combat.targetStats.buffManager, "Decisive Strike"));
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}