using UnityEngine;

public class AttackDamageBuff : Buff
{
    public string uniqueKey;
    public AttackDamageBuff(float duration, BuffManager manager, string source, int AD, string uniqueKey) : base(manager)
    {
        manager.stats.AD += AD;
        value = AD;
        base.duration = duration;
        base.source = source;
        this.uniqueKey = uniqueKey;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {AD} Attack Damage from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        if (!paused) duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value} Extra Attack AD From {source}!");
        manager.stats.AD -= value;
        manager.buffs.Remove(uniqueKey);
    }
}
