using Simulator.Combat;

public class KogMawAACheck : Check
{
    public KogMawAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("BioArcaneBarrage", out Buff value))
        {
            damage += value.value;
            combat.wSum += value.value * damage / 100;
            combat.myUI.abilitySum[1].text = combat.wSum.ToString();
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}