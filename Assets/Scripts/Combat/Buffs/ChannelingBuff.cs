using UnityEngine;

public class ChannelingBuff : Buff
{
    public string uniqueKey;
    public ChannelingBuff(float duration, BuffManager manager, string source, string uniqueKey) : base(manager)
    {
        base.duration = float.MaxValue;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Channels {source}");
        this.uniqueKey = uniqueKey;
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} No Longer Channels {source}");
        manager.combat.StopChanneling(uniqueKey);
        manager.buffs.Remove("Channeling");
    }
}