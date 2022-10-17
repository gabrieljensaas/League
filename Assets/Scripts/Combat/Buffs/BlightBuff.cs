using UnityEngine;

public class BlightBuff : Buff
{
    public BlightBuff(float duration, BuffManager manager, string source, int value = 1) : base(manager)
    {
        base.value = value;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Got Blight By {source} For {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Blighted By {source}");
        manager.buffs.Remove("Blight");
    }
}