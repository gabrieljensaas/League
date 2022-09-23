using UnityEngine;

public class VitalsBuff : Buff
{
    public float activationTime;
    public bool isActive;

    public VitalsBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Vitals for {duration} Seconds Because of {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();

        if (!isActive && duration <= activationTime) isActive = true;
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} removed Vitals!");
        manager.buffs.Remove("Vitals");
        manager.buffs.Remove("VitalsGrandChallenge");
    }
}