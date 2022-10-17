using UnityEngine;

public class ImmuneToCCBuff : Buff
{
    private string uniqueKey;
    public ImmuneToCCBuff(float duration, BuffManager manager, string source, string uniqueKey) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Is Immune To CC Because Of {source} For {base.duration:F3} Seconds!");
        this.uniqueKey = uniqueKey;
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Immune To CC By {source}!");
        manager.buffs.Remove(uniqueKey);
    }
}