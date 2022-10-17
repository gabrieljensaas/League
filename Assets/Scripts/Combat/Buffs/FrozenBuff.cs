using UnityEngine;

public class FrozenBuff : Buff
{
    public FrozenBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Got Frozen By {source} For {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Frozen By {source}");
        manager.buffs.Remove("Frozen");
    }
}