using UnityEngine;

public class AmumuCurseBuff : Buff
{
    public AmumuCurseBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has curse for {duration}!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();

    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Curse!");
        manager.buffs.Remove("AmumuCurse");
    }
}