using Simulator.Combat;

public class CheckIfSleptByZoe : Check
{
    public CheckIfSleptByZoe(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("Sleep", out Buff value))
        {
            combat.targetCombat.UpdateAbilityTotalDamage(ref combat.targetCombat.eSum, 2, damage, "Sleepy Trouble Bubble", SkillDamageType.True);
            value.Kill();
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}