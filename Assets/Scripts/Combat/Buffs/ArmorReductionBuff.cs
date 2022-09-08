using UnityEngine;

public class ArmorReductionBuff : Buff
{
    public string uniqueKey;
    public ArmorReductionBuff(float duration, BuffManager manager, string source, float reduction, string uniqueKey) : base(manager)
    {
        base.value = reduction;
        base.duration = duration;
        manager.stats.armor *= (100 - reduction) / 100;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has {reduction}% Percent Reduced Armor for {duration} Seconds!");
        this.uniqueKey = uniqueKey;
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value}% Percent Reduced Armor From {source}!");
        manager.stats.armor *= 100 / (100 - value);
        manager.buffs.Remove(uniqueKey);
    }
}