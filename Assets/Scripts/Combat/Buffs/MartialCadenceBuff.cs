using UnityEngine;

public class MartialCadenceBuff : Buff
{
    public MartialCadenceBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} was affected by Martial Cadence for {duration}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} doesn't have Martial Cadence anymore!");
        manager.buffs.Remove("MartialCadence");
    }
}