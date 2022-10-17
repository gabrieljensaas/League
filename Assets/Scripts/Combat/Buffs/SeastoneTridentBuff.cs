using UnityEngine;

public class SeastoneTridentBuff : Buff
{
    public SeastoneTridentBuff(float duration, BuffManager manager, string source, int value = 1) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        base.value = value;
        manager.simulationManager.ShowText($"{manager.stats.name} Got Seastone Trident By {source} For {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer Seastone Trident By {source}");
        manager.buffs.Remove("SeastoneTrident");
    }
}