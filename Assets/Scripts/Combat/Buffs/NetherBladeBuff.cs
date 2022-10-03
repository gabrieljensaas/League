using UnityEngine;

public class NetherBladeBuff : Buff
{
    public NetherBladeBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Nether Blade from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer Nether Blade from {source}!");
        manager.buffs.Remove("NetherBlade");
    }
}