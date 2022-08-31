using UnityEngine;

public class LifeStealBuff : Buff
{
    public string uniqueKey;
    public LifeStealBuff(float duration, BuffManager manager, string source, float lifesteal, string uniqueKey) : base(manager)
    {
        manager.stats.lifesteal += lifesteal;
        this.value = lifesteal;
        base.duration = duration;
        base.source = source;
        this.uniqueKey = uniqueKey;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {lifesteal:F3} Lifesteal from {source} for {duration:F3} Seconds!");
    }

    public override void Update()
    {
        if (!paused) duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value} Extra Lifesteal From {source}!");
        manager.stats.lifesteal -= value;
        manager.buffs.Remove(uniqueKey);
    }
}