using UnityEngine;

public class KnockdownBuff : Buff
{
    public KnockdownBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration; //effect is instantaneous
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} is Knockdowned by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Knockdowned by {source}!");
        manager.buffs.Remove("Knockdown");
    }
}