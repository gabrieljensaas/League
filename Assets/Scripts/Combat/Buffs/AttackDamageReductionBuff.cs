using UnityEngine;

public class AttackDamageReductionBuff : Buff
{
    public string uniqueKey;
    public AttackDamageReductionBuff(float duration, BuffManager manager, string source, int AD, string uniqueKey) : base(manager)
    {
        manager.stats.AD -= AD;
        value = AD;
        base.duration = duration;
        base.source = source;
        this.uniqueKey = uniqueKey;
        manager.simulationManager.ShowText($"{manager.stats.name} Lost {AD} Attack Damage from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        if (!paused) duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value}  Attack AD Lost From {source}!");
        manager.stats.AD += value;
        manager.buffs.Remove(uniqueKey);
    }
}
