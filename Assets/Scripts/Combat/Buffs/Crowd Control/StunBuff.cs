using UnityEngine;

public class StunBuff : Buff
{
    public StunBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration * (100 - manager.stats.tenacity) / 100;
        base.source = source;

        if (manager.buffs.TryGetValue("Channeling", out Buff value))
            value.Kill();

        manager.simulationManager.ShowText($"{manager.stats.name} is Stunned by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Stunned By {source}!");
        manager.buffs.Remove("Stun");
    }
}