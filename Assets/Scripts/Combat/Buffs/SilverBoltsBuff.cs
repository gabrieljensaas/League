using UnityEngine;

public class SilverBoltsBuff : Buff
{
    public SilverBoltsBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        this.value = 1;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has 1 Stack of Silver Bolts from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer Silver Bolts from {source}!");
        manager.buffs.Remove("SilverBolts");
    }
}