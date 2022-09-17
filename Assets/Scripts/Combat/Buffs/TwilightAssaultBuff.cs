using UnityEngine;

public class TwilightAssaultBuff : Buff
{
    public TwilightAssaultBuff(float duration, BuffManager manager, string source, float value) : base(manager)
    {
        this.value = value;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has Twilight Assault by {source} for {duration} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer any Twilight Assault!");
        manager.buffs.Remove("TwilightAssault");
    }
}