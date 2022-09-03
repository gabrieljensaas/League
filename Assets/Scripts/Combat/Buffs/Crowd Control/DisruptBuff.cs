using UnityEngine;

public class DisruptBuff : Buff
{
    public DisruptBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration; //effect is instantaneous
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} is Disrupted by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Disrupted by {source}!");
        manager.buffs.Remove("Disrupt");
    }
}