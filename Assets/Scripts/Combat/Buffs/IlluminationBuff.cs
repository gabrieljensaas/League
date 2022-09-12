using UnityEngine;

public class IlluminationBuff : Buff
{
    public IlluminationBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} got illuminated by {source} for {duration} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer illuminated By {source}");
        manager.buffs.Remove("Illumination");
    }
}
