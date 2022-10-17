using UnityEngine;

public class AkaliPassiveBuff : Buff
{
    public AkaliPassiveBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Swinging Kama for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Doesn't Have Any Swinging Kama!");
        manager.buffs.Remove("AkaliPassive");
    }
}