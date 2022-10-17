using Simulator.Combat;

public class CheckMoltenShield : Check
{
    public CheckMoltenShield(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.shields.ContainsKey("Molten Shield") && combat.myStats.eLevel > 0)
        {
            combat.UpdateAbilityTotalDamage(ref combat.eSum, 2, combat.myStats.eSkill[0], combat.myStats.eLevel, combat.eKeys[1]);
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}