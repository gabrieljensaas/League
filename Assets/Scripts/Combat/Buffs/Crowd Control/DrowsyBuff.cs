using UnityEngine;

public class DrowsyBuff : Buff
{
    private float sleepDuration;

    public DrowsyBuff(float duration, BuffManager manager, string source, float sleepDuration) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        this.sleepDuration = sleepDuration;

        manager.simulationManager.ShowText($"{manager.stats.name} is Drowsied by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Drowsied by {source}!");
        manager.buffs.Add("Sleep", new SleepBuff(sleepDuration, manager, source));
        manager.buffs.Remove("Drowsy");
    }
}