using UnityEngine;

public class UndyingRageBuff : Buff
{
    public UndyingRageBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has Undying Rage for {duration}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Undying Rage!");
        manager.buffs.Remove("UndyingRage");
    }
}