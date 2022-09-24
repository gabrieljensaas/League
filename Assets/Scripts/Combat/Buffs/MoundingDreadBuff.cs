using UnityEngine;

public class MoundingDreadBuff : Buff
{
    public MoundingDreadBuff(float duration, BuffManager manager) : base(manager)
    {
        value = 1;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Gained a Stack");
    }

    public override void Update()
    {
    }
    public override void Kill()
    {
        duration = -Time.deltaTime;
        manager.simulationManager.ShowText($"{manager.stats.name} Doesn't Have Any Stack Anymore!");
        manager.buffs.Remove("MoundingDread");
    }
}