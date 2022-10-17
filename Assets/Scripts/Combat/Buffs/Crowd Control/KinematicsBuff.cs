using UnityEngine;

public class KinematicsBuff : Buff
{
    public KinematicsBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration; //effect is instantaneous
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} is Dragged by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Dragged by {source}!");
        manager.buffs.Remove("Kinematics");
    }
}