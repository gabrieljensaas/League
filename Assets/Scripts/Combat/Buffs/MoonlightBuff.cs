using UnityEngine;

public class MoonlightBuff : Buff
{
    public MoonlightBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has been Moonlight from {source}!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Moonlight from {source}!");
        manager.buffs.Remove("Moonlight");
    }
}