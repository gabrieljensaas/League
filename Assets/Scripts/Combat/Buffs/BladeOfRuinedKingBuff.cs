using UnityEngine;

public class BladeOfRuinedKingBuff : Buff
{
    public BladeOfRuinedKingBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} has been marked by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has mark!");
        manager.buffs.Remove("BladeOfRuinedKing");
    }
}
