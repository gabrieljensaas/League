using UnityEngine;

public class ShieldBuff : Buff
{
    public string uniqueKey;
    public ShieldBuff(float duration, BuffManager manager, string source, float shield, string uniqueKey) : base(manager)
    {
        value = shield;
        base.duration = float.MaxValue;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {shield} Shield from {source} for {duration} Seconds!");
        this.uniqueKey = uniqueKey;
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer Any Shield!");
        manager.buffs.Remove(uniqueKey);
    }
}