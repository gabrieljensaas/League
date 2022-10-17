using UnityEngine;

public class HarrierVulnerableBuff : Buff
{
    public HarrierVulnerableBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name}  is Vulnerable from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Vulnerable From {source}!");
        manager.buffs.Remove("HarrierVulnerable");
    }
}
