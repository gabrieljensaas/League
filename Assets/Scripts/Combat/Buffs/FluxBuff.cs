using UnityEngine;

public class FluxBuff : Buff
{
    private Ryze ryze;
    public FluxBuff(float duration, BuffManager manager) : base(manager)
    {
        base.duration = duration;
        manager.simulationManager.ShowText($"{manager.stats.name} has Flux For {duration} Seconds!");
        ryze.hasFlux = true;
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} No Longer has Flux");
        manager.buffs.Remove("FluxBuff");
        ryze.hasFlux = false;
    }
}