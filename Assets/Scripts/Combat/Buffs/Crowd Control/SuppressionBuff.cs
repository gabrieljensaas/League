using UnityEngine;

public class SuppressionBuff : Buff
{
    public SuppressionBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration * (100 - manager.stats.tenacity) / 100;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} is Suppressed by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Suppressed By {source}!");
        manager.buffs.Remove("Suppression");
    }
}