using UnityEngine;

public class DeadlyFlourishBuff : Buff
{
    public DeadlyFlourishBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has a deadly flourish mark by {source} for {duration} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has a deadly flourish mark {source}");
        manager.buffs.Remove("Deadly Flourish Mark");
    }
}
