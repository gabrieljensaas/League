using Simulator.Combat;

public class CheckMoltenShield : Check
{
    public CheckMoltenShield(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (combat.myStats.buffManager.shields.ContainsKey("Molten Shield"))
        {
            combat.UpdateAbilityTotalDamage(ref combat.eSum, 2, combat.myStats.eSkill[0], 4, combat.eKeys[1]);
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}