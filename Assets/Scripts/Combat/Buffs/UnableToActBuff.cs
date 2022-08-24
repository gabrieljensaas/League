using UnityEngine;

public class UnableToActBuff : Buff
{
    public UnableToActBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Cant Act for {duration} Seconds Because of {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Can Act Now!");
        manager.buffs.Remove("UnableToAct");
    }
}