using UnityEngine;

public class PoisonBuff : Buff
{
    private Cassiopeia cassiopeia;
    public PoisonBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Got poisoned By {source} for {duration} seconds!");
        cassiopeia.isPoisoned = true;
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer poisoned by {source}");
        manager.buffs.Remove("PoisonBuff");
        cassiopeia.isPoisoned = false;
    }
}
