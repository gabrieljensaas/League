using UnityEngine;

public class ShieldBuff : Buff
{
    public string uniqueKey;
    public bool decaying = false;
    public ShieldBuff(float duration, BuffManager manager, string source, float shield, string uniqueKey, bool decaying = false) : base(manager)
    {
        this.decaying = decaying;
        value = shield;
        base.duration = float.MaxValue;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {shield} Shield from {source} for {duration} Seconds!");
        this.uniqueKey = uniqueKey;
        this.decaying = decaying;
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (decaying) value -= (Time.deltaTime / duration) * value;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer Shield From {source}!");
        manager.buffs.Remove(uniqueKey);
    }
}