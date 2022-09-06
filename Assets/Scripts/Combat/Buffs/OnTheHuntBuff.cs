using UnityEngine;

public class OnTheHuntBuff : Buff
{
    public OnTheHuntBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} is On The Hunt for {duration} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer On The Hunt!");
        manager.buffs.Remove("OnTheHunt");
    }
}