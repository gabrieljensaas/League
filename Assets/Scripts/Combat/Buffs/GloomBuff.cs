using UnityEngine;

public class GloomBuff : Buff
{
    public GloomBuff(float duration, BuffManager manager) : base(manager)
    {
        base.duration = duration;
        manager.simulationManager.ShowText($"{manager.stats.name} has Gloom For {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} No Longer has Gloom");
        manager.buffs.Remove("Gloom");
    }
}