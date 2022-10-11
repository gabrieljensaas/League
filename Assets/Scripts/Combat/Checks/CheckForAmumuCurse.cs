using Simulator.Combat;

public class CheckForAmumuCurse : Check
{
    public CheckForAmumuCurse(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        if (damage.value > 0 && damage.damageType == SkillDamageType.Spell && combat.MyBuffManager.buffs.ContainsKey("AmumuCurse"))
        {
            combat.targetCombat.UpdateAbilityTotalDamage(ref combat.targetCombat.pSum, 5, new Damage(damage.value * 0.1f, SkillDamageType.True), "Cursed Touch");
        }
        return damage;
    }
}