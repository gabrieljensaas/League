using UnityEngine;

public class RootBuff : Buff
{
    public RootBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} is Rooted by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Rooted by {source}!");
        manager.buffs.Remove("Root");
    }
}