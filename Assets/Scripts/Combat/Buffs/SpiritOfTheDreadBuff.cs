using UnityEngine;

public class SpiritOfTheDreadBuff : Buff
{
    public SpiritOfTheDreadBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has {source}!");
        manager.buffs.Remove("SpiritOfTheDread");
    }
}