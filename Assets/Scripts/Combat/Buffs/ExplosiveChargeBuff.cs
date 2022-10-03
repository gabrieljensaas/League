using UnityEngine;

public class ExplosiveChargeBuff : Buff
{
    public ExplosiveChargeBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has an explosive charge by {source} for {duration} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.combat.targetCombat.UpdateAbilityTotalDamage(ref manager.combat.targetCombat.eSum, 2, new Damage(manager.stats.eSkill[0].UseSkill(4, manager.combat.eKeys[0], manager.stats, manager.combat.targetStats) + (manager.stats.eSkill[0].UseSkill(4, manager.combat.eKeys[1], manager.stats, manager.combat.targetStats) * value), SkillDamageType.Phyiscal), source);

        manager.simulationManager.ShowText($"{manager.stats.name} no longer has an explosive charge by {source}");
        manager.buffs.Remove("Explosive Charge");
    }
}
