using UnityEngine;

public class LotusTrapBuff : Buff
{
    public LotusTrapBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has been lotus trapped for {duration} seconds because of {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.combat.targetCombat.UpdateAbilityTotalDamage(ref manager.combat.targetCombat.eSum, 2, 160, source, SkillDamageType.Spell);

        manager.simulationManager.ShowText($"{manager.stats.name} is not lotus trapped anymore!");
        manager.buffs.Remove("Lotus Trap");
    }
}
