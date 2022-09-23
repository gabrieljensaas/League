using Simulator.Combat;

public class KayleAACheck : Check
{
    public KayleAACheck(ChampionCombat ccombat) : base(ccombat)
    {

    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("Starfire SpellBlade", out Buff value))
        {
            damage += value.value;
            combat.eSum += value.value;
            combat.myUI.abilitySum[0].text = combat.eSum.ToString();
            combat.myStats.buffManager.buffs.Remove("Starfire SpellBlade");
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}
