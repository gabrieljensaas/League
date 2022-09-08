using UnityEngine;

public class DamageReductionPercentBuff : Buff
{
    public DamageReductionPercentBuff(float duration, BuffManager manager, string source, float reduction) : base(manager)
    {
        this.value = reduction;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {reduction}% Percent Damage Reduction from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value}% Percent Damage Reduction From {source}!");
        manager.buffs.Remove("DamageReductionPercent");
    }
}
