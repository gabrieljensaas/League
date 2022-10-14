using Simulator.Combat;

public class KassadinAACheck : Check
{
    public KassadinAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }

    public override Damage Control(Damage damage)
    {
        if (combat.MyBuffManager.buffs.ContainsKey("NetherBlade"))
        {
            if (combat.UpdateAbilityTotalDamage(ref combat.wSum, 1, combat.WSkill(), combat.myStats.wLevel, combat.wKeys[0]) <= 0) damage.value = 0;
            return damage;
        }
        if (combat.myStats.qLevel > 0) combat.UpdateAbilityTotalDamage(ref combat.wSum, 1, new Damage(20 + (combat.myStats.AP * 0.1f), SkillDamageType.Spell, SkillComponentTypes.ProcDamage), combat.WSkill().basic.name);
        return damage;
    }
}