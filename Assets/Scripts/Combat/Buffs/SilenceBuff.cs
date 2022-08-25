using UnityEngine;

public class SilenceBuff : Buff
{
    public SilenceBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration * (100 - manager.stats.tenacity) / 100;
        base.source = source;
        if(manager.buffs.TryGetValue("Channeling", out Buff value))
        {
            value.Kill();
        }
        manager.simulationManager.ShowText($"{manager.stats.name} Got Silenced By {source} For {base.duration:F3} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if(duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Silenced By {source}!");
        manager.buffs.Remove("Silence");
    }
}