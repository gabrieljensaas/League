using UnityEngine;

public class DisarmBuff : Buff
{
    public DisarmBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration * (100 - manager.stats.tenacity) / 100;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Got Disarmed By {source} For {base.duration:F3} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if(duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Disarmed By {source}!");
        manager.buffs.Remove("Disarm");
    }
}