using UnityEngine;

public class ToxicShotBuff : Buff
{
    float tickTimer = 0;

    public ToxicShotBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has Poison for {duration} seconds from {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();

        tickTimer += Time.deltaTime;
        if (tickTimer >= 1)
        {
            manager.combat.targetCombat.UpdateAbilityTotalDamage(ref manager.combat.targetCombat.eSum, 2, manager.combat.targetStats.eSkill[1], 4, manager.combat.targetCombat.eKeys[0]);
            tickTimer = 0;
        }
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Poison from {source}!");
        manager.buffs.Remove("ToxicShot");
    }
}