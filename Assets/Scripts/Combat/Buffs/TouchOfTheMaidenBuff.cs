using UnityEngine;

public class TouchOfTheMaidenBuff : Buff
{
    public TouchOfTheMaidenBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has Touch of the Maiden!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Touch of the Maiden!");
        manager.buffs.Remove("TouchOfTheMaiden");
    }
}