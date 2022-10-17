using UnityEngine;

public class AbilityPowerBuff : Buff
{
    public string uniqueKey;
    public AbilityPowerBuff(float duration, BuffManager manager, string source, int AP, string uniqueKey) : base(manager)
    {
        manager.stats.AP += AP;
        value = AP;
        base.duration = duration;
        base.source = source;
        this.uniqueKey = uniqueKey;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {AP} Ability Power from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        if (!paused) duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value} Extra AP From {source}!");
        manager.stats.AP -= value;
        manager.buffs.Remove(uniqueKey);
    }
}