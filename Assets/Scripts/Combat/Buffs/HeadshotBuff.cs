using UnityEngine;

public class HeadshotBuff : Buff
{
    public HeadshotBuff(BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has {value} Stack of Headshot for!");
    }

    public override void Update()
    {
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Used Her Headshot Stacks!");
        manager.buffs.Remove("Headshot");
    }
}