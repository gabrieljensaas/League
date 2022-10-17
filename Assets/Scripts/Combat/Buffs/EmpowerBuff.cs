using UnityEngine;

public class EmpowerBuff : Buff
{
    public EmpowerBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} gained Empower from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Empower from {source}!");
        manager.buffs.Remove("Empower");
    }
}
