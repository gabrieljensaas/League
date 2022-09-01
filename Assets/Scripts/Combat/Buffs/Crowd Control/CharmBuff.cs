using UnityEngine;

public class CharmBuff : Buff
{
    public CharmBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration * (100 - manager.stats.tenacity) / 100;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} is Charmed by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Charmed by {source}!");
        manager.buffs.Remove("Charm");
    }
}