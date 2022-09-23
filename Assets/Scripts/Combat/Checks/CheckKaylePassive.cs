using Simulator.Combat;

public class CheckKaylePassive : Check
{
    public CheckKaylePassive(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override float Control(float damage)
    {
        float missingHpPercent = (combat.myStats.maxHealth - combat.myStats.currentHealth) / combat.myStats.maxHealth;
        var multiplier = missingHpPercent >= 0.7f ? 1 : missingHpPercent / 0.7f;
        var newSpeed = multiplier * (0.6f + (0.4f / 17 * (combat.myStats.level - 1)));
        var newLifesteal = multiplier * (0.07f + (combat.myStats.level / 100));
        if (combat.myStats.buffManager.buffs.TryGetValue(combat.myStats.passiveSkill.skillName, out Buff value))
        {
            combat.attackCooldown *= combat.myStats.attackSpeed / (combat.myStats.attackSpeed - value.value);
            combat.myStats.attackSpeed -= value.value;
            combat.attackCooldown *= combat.myStats.attackSpeed / (combat.myStats.attackSpeed + newSpeed);
            combat.myStats.attackSpeed += newSpeed;
            value.value = newSpeed;
        }
        else
        {
            combat.myStats.buffManager.buffs.Add(combat.myStats.passiveSkill.skillName, new AttackSpeedBuff(float.MaxValue, combat.myStats.buffManager, combat.myStats.passiveSkill.skillName, newSpeed, combat.myStats.passiveSkill.skillName));
        }
        if (combat.myStats.buffManager.buffs.TryGetValue(combat.myStats.passiveSkill.skillName + " ", out Buff buff))
        {
            combat.myStats.lifesteal -= buff.value;
            combat.myStats.lifesteal += newLifesteal;
            buff.value = newLifesteal;
        }
        else
        {
            combat.myStats.buffManager.buffs.Add(combat.myStats.passiveSkill.skillName + " ", new LifeStealBuff(float.MaxValue, combat.myStats.buffManager, combat.myStats.passiveSkill.skillName, newLifesteal, combat.myStats.passiveSkill.skillName + " "));
        }
        return damage;
    }
}