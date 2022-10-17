using UnityEngine;

public class RunicBladeBuff : Buff
{
    public RunicBladeBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has {value} Runic Blade for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Doesn't Have Any Runic Blade!");
        manager.buffs.Remove("RunicBlade");
    }
}