using UnityEngine;

public class ChilledBuff : Buff
{
    private Anivia anivia;
    public ChilledBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Got Chilled By {source} for {duration} seconds!");
        anivia.isChilled = true;
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Chilled by {source}");
        manager.buffs.Remove("ChilledBuff");
        anivia.isChilled = false;
    }
}
