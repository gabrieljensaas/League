using UnityEngine;

public class SlowBuff : Buff
{
    public SlowBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration * (100 - manager.stats.tenacity) / 100;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} is Slowed by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Slowed by {source}!");
        manager.buffs.Remove("Slow");
    }
}