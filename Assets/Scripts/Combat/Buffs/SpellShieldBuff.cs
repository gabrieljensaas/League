using UnityEngine;

public class SpellShieldBuff : Buff
{
    public SpellShieldBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has Spell Shield for {duration} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} has no more Spell Shield!");
        manager.buffs.Remove("SpellShield");
    }
}
