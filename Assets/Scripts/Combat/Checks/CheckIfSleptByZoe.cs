using Simulator.Combat;

public class CheckIfSleptByZoe : Check
{
    public CheckIfSleptByZoe(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("Sleep", out Buff value))
        {
            damage.damageType = SkillDamageType.True;
            combat.targetCombat.UpdateAbilityTotalDamage(ref combat.targetCombat.eSum, 2, damage, "Sleepy Trouble Bubble");
            value.Kill();
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}