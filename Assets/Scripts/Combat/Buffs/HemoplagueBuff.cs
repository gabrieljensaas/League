using UnityEngine;

public class HemoplagueBuff : Buff
{
    public HemoplagueBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Got Hemoplague By {source} For {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Hemoplagued By {source}");
        manager.buffs.Remove("Hemoplague");
    }
}