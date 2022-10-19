using UnityEngine;

public class ExplosiveChargeBuff : Buff
{
    public ExplosiveChargeBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has an explosive charge by {source} for {duration} seconds!");
        value = 1;
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.combat.targetCombat.UpdateAbilityTotalDamage(ref manager.combat.targetCombat.eSum, 2, new Damage(manager.combat.targetStats.eSkill[0].UseSkill(4, manager.combat.targetCombat.eKeys[1], manager.combat.targetStats, manager.stats) * value, SkillDamageType.Phyiscal), source);

        manager.simulationManager.ShowText($"{manager.stats.name} no longer has an explosive charge by {source}");
        manager.buffs.Remove("Explosive Charge");
    }
}