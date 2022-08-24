using UnityEngine;

public class DoubleStrikeBuff : Buff
{
    public DoubleStrikeBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has {value} Stack of Double Strike for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Doesn't Have Any Double Strike Stacks Anymore!");
        manager.buffs.Remove("DoubleStrike");
    }
}