using UnityEngine;

public class AirborneBuff : Buff
{
    public AirborneBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;

        if (manager.buffs.TryGetValue("Channeling", out Buff value))
            value.Kill();

        manager.simulationManager.ShowText($"{manager.stats.name} is Airborned by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Airborned by {source}!");
        manager.buffs.Remove("Airborne");
    }
}