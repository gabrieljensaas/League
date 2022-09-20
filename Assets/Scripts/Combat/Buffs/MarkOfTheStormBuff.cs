using UnityEngine;

public class MarkOfTheStormBuff : Buff
{
    public MarkOfTheStormBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Gained a Stacks of Mark From {source}!");
    }

    public override void Update()
    {
    }
    public override void Kill()
    {
        duration=- Time.deltaTime;
        manager.simulationManager.ShowText($"{manager.stats.name} Doesn't Have Any Mark Stacks Anymore!");
        manager.buffs.Remove("MarkOfTheStorm");
    }
}