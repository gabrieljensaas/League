using UnityEngine;

public class BrittleBuff : Buff
{
    public BrittleBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        manager.stats.tenacity -= 30;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained Brittle from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.stats.tenacity += 30;
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer Brittle from {source}!");
        manager.buffs.Remove("Brittle");
    }
}