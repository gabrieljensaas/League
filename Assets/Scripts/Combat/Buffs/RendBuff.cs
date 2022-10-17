using UnityEngine;

public class RendBuff : Buff
{
    public RendBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.source = source;
        base.duration = duration;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Gained a Stack of Rend From {source}!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Doesn't Have Any Rend Stacks Anymore!");
        manager.buffs.Remove("Rend");
    }
}