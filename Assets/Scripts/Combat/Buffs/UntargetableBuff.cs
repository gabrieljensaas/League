using UnityEngine;

public class UntargetableBuff : Buff
{
    public UntargetableBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Cant Be Targetable for {duration} Seconds Because of {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Can Be Targetable Now!");
        manager.buffs.Remove("Untargetable");
    }
}