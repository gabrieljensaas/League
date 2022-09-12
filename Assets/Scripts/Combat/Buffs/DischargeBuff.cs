using UnityEngine;

public class DischargeBuff : Buff
{
    public DischargeBuff(float duration, BuffManager manager, string source, int value = 1) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Got Discarge By {source} For {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer Discharge By {source}");
        manager.buffs.Remove("Discharge");
    }
}