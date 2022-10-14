using UnityEngine;

public class DirtyFightingBuff : Buff
{
    public DirtyFightingBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = float.MaxValue;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Dirty Fighting Stack From {source}");
        this.value = 1;
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} No Longer Has Dirty Fighting Stacks!");
        manager.buffs.Remove("DirtyFighting");
    }
}