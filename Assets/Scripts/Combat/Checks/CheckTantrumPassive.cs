using Simulator.Combat;

public class CheckTantrumPassive : Check
{
    public CheckTantrumPassive(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (damage.damageType == SkillDamageType.Phyiscal)
        {
            var reduction = combat.ESkill().UseSkill(combat.myStats.eLevel, combat.eKeys[0], combat.myStats, combat.targetStats);
            damage.value -= damage.value * 0.5f > reduction ? reduction : damage.value * 0.5f;
        }
        if (damage.skillComponentType == SkillComponentTypes.OnHit)
        {
            combat.myStats.eCD -= 0.5f;
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}