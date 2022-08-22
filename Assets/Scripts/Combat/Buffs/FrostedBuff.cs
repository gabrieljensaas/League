using UnityEngine;

public class FrostedBuff : Buff
{
    public FrostedBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Got Frosted By {source} For {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if(duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Frosted By {source}");
        manager.buffs.Remove("Frosted");
    }
}