using UnityEngine;

public class EmpoweredTumbleBuff : Buff
{
    public EmpoweredTumbleBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained Empowered Tumble from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer Empowered Tumble From {source}!");
        manager.buffs.Remove("EmpoweredTumble");
    }
}
