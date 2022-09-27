using UnityEngine;

public class DemacianStandardBuff : Buff
{
    public DemacianStandardBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has a flag for {duration} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} doesn't have a flag anymore!");
        manager.buffs.Remove("DemacianStandardFlag");
    }
}