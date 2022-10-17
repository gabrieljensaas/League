using UnityEngine;

public class DisplacementImmunityBuff : Buff
{
    public DisplacementImmunityBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has Displacement Immunity for {duration} seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} doesn't have Displacement Immunity anymore!");
        manager.buffs.Remove("DisplacementImmunity");
    }
}