using UnityEngine;

public class PhaseDiveBuff : Buff
{
    public PhaseDiveBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} got Phase Dive by {source} for {duration} seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Phase Dive By {source}");
        manager.buffs.Remove("PhaseDive");
    }
}