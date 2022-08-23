using UnityEngine;

public class PyromaniaBuff : Buff
{
    public PyromaniaBuff(BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Gained a Stacks of Pyromania From {source}!");
    }

    public override void Update()
    {
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Doesn't Have Any Pyromania Stacks Anymore!");
        manager.buffs.Remove("Pyromania");
    }
}