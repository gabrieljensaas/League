using UnityEngine;

public class MoonsilverBladeBuff : Buff
{
    public MoonsilverBladeBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has Moonsilver Blade!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Moonsilver Blade!");
        manager.buffs.Remove("MoonsilverBlade");
    }
}