using UnityEngine;

public class ArmorBuff : Buff
{
    public string uniqueKey;
    public ArmorBuff(float duration, BuffManager manager, string source, float armor, string uniqueKey) : base(manager)
    {
        manager.stats.armor += armor;
        value = armor;
        base.duration = duration;
        base.source = source;
        this.uniqueKey = uniqueKey;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {armor} Armor from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        if (!paused) duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value} Extra Armor From {source}!");
        manager.stats.armor -= value;
        manager.buffs.Remove(uniqueKey);
    }
}
