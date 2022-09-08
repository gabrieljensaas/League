using UnityEngine;

public class LightslingerBuff : Buff
{
    public LightslingerBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Gained Lightslinger from {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Used Her Lightslinger from {source}!");
        manager.buffs.Remove("Lightslinger");
    }
}