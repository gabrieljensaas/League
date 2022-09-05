using UnityEngine;

public class SpinningAxeBuff : Buff
{
    public SpinningAxeBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.source = source;
        base.duration = duration;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Gained a Spinning Axe From {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Doesn't Have Any Spinning Axe Anymore!");
        manager.buffs.Remove("SpinningAxe");
    }
}