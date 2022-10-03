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

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes skillComponenetType = SkillComponentTypes.None)
    {
        if (combat.myStats.qLevel > 0) combat.UpdateAbilityTotalDamage(ref combat.wSum, 1, 20 + (combat.myStats.AP * 0.1f), combat.WSkill().basic.name, SkillDamageType.Spell, SkillComponentTypes.ProcDamage);
        return damage;
    }
}