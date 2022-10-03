using Simulator.Combat;

public class CheckTryndamereUndyingRage : Check
{
    public CheckTryndamereUndyingRage(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (combat.myStats.buffManager.buffs.ContainsKey("UndyingRage") && combat.myStats.currentHealth - damage < combat.myStats.rSkill[0].UseSkill(2, combat.rKeys[1], combat.myStats, combat.targetStats))
            damage = combat.myStats.currentHealth + combat.myStats.rSkill[0].UseSkill(2, combat.rKeys[1], combat.myStats, combat.targetStats);

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}