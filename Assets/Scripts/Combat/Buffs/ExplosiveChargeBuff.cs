using System.Collections;
using System.Collections.Generic;
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
        manager.combat.targetCombat.UpdateAbilityTotalDamage(ref manager.combat.targetCombat.eSum, 2, Constants.GetTristanaExplosiveChargeByLevel(5, (int)value), source, SkillDamageType.Phyiscal);

        manager.simulationManager.ShowText($"{manager.stats.name} no longer has an explosive charge by {source}");
        manager.buffs.Remove("Explosive Charge");
    }
}
