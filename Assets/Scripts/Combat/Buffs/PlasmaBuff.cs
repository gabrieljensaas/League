using UnityEngine;

public class PlasmaBuff : Buff
{
    public PlasmaBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        this.value = 1;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has 1 Stack of Plasma from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer Plasma Bolts from {source}!");
        manager.buffs.Remove("Plasma");
    }
}