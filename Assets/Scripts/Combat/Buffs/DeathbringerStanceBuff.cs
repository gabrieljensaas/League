using UnityEngine;

public class DeathbringerStanceBuff : Buff
{
    public DeathbringerStanceBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = float.MaxValue;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has DeathBringer Stance!");
    }

    public override void Update()
    {

    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer DeathBringer Stance!");
        manager.buffs.Remove("DeathbringerStance");
    }
}