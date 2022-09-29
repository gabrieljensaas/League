using UnityEngine;

public class OrganicDestructionBuff : Buff
{
    public OrganicDestructionBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Gained a Stack of Destruction From {source}!");
    }

    public override void Update()
    {
    }
    public override void Kill()
    {
        duration = -Time.deltaTime;
        manager.simulationManager.ShowText($"{manager.stats.name} Doesn't Have Any stack Anymore!");
        manager.buffs.Remove("OrganicDestruction");
    }
}