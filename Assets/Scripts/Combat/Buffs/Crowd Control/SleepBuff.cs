using UnityEngine;

public class SleepBuff : Buff
{
    private bool isActive;

    public bool IsActive => isActive;

    public SleepBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration * (100 - manager.stats.tenacity) / 100;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} is Asleep by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Asleep by {source}!");
        manager.buffs.Remove("Sleep");
    }
}