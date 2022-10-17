using UnityEngine;

public class PermaFrostBuff : Buff
{
    public PermaFrostBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Gained a PermaFrost From {source}!");
    }

    public override void Update()
    {
    }
    public override void Kill()
    {
        duration = -Time.fixedDeltaTime;
        manager.simulationManager.ShowText($"{manager.stats.name} Doesn't Have Any PermaFrost Anymore!");
        manager.buffs.Remove("PermaFrost");
    }
}