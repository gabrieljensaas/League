using UnityEngine;

public class GrievousWoundsBuff : Buff
{
    public string uniqueKey;
    public GrievousWoundsBuff(float duration, BuffManager manager, string source, float woundsPercent, string uniqueKey) : base(manager)
    {
        this.value = woundsPercent;
        manager.stats.grievouswounds = woundsPercent;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {woundsPercent}% Percent Grievous Wounds from {source} for {duration} Seconds!");
        this.uniqueKey = uniqueKey;
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.stats.grievouswounds -= value;
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value}% Percent Griveous Wounds from {source}!");
        manager.buffs.Remove(uniqueKey);
    }
}