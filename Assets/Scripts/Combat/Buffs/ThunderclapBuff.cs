using UnityEngine;

public class ThunderclapBuff : Buff
{
    public ThunderclapBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has Thunderclap for {duration} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Thunderclap!");
        manager.buffs.Remove("ThunderclapBuff");
    }
}