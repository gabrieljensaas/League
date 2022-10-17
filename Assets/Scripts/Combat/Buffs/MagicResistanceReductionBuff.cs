using UnityEngine;

public class MagicResistanceReductionBuff : Buff
{
    public string uniqueKey;
    public MagicResistanceReductionBuff(float duration, BuffManager manager, string source, float reduction, string uniqueKey) : base(manager)
    {
        base.value = reduction;
        base.duration = duration;
        manager.stats.spellBlock *= (100 - reduction) / 100;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has {reduction}% Percent Reduced Armor for {duration} Seconds!");
        this.uniqueKey = uniqueKey;
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value}% Percent Reduced Armor From {source}!");
        manager.stats.spellBlock *= 100 / (100 - value);
        manager.buffs.Remove(uniqueKey);
    }
}