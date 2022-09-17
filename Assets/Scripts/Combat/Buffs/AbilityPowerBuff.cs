using UnityEngine;

public class AbilityPowerBuff : Buff
{
    public string uniqueKey;
    public AbilityPowerBuff(float duration, BuffManager manager, string source, int AP, string uniqueKey) : base(manager)
    {
        manager.stats.AP += manager.stats.AP * AP * 0.01f;
        value = manager.stats.AP * AP * 0.01f;
        base.duration = duration;
        base.source = source;
        this.uniqueKey = uniqueKey;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {AP} Percentage Ability Power from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        if (!paused) duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value} Extra Percentage AP From {source}!");
        manager.stats.AP -= value;
        manager.buffs.Remove(uniqueKey);
    }
}
