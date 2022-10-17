using UnityEngine;

public class TenacityBuff : Buff
{
    public string uniqueKey;
    public TenacityBuff(float duration, BuffManager manager, string source, float tenacity, string uniqueKey) : base(manager)
    {
        this.value = tenacity;
        manager.stats.tenacity = tenacity;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {tenacity}% Percent Tenacity from {source} for {duration} Seconds!");
        this.uniqueKey = uniqueKey;
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.stats.tenacity -= value;
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value}% Percent Extra Tenacity from {source}!");
        manager.buffs.Remove(uniqueKey);
    }
}