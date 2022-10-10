using UnityEngine;

public class MagicResistanceBuff : Buff
{
    public string uniqueKey;
    public MagicResistanceBuff(float duration, BuffManager manager, string source, float magicResistance, string uniqueKey) : base(manager)
    {
        manager.stats.spellBlock += magicResistance;
        value = magicResistance;
        base.duration = duration;
        base.source = source;
        this.uniqueKey = uniqueKey;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {magicResistance} Magic Resistance from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        if (!paused) duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value} Extra Magic Resistance From {source}!");
        manager.stats.spellBlock -= value;
        manager.buffs.Remove(uniqueKey);
    }
}
