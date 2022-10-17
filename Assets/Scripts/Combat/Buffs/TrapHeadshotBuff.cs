using UnityEngine;

public class TrapHeadshotBuff : Buff
{
    public TrapHeadshotBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Gained Headshot From Trap!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Used Her Headshot From Trap!");
        manager.buffs.Remove("TrapHeadshot");
    }
}