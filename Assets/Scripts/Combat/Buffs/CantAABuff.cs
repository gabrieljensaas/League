using UnityEngine;

public class CantAABuff : Buff
{
    public CantAABuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Cant Auto Attack for {duration} Seconds Because of {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Can Auto Attack Now!");
        manager.buffs.Remove("CantAA");
    }
}