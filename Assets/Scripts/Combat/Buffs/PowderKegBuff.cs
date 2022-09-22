using UnityEngine;

public class PowderKegBuff : Buff
{
    private float activationTime;
    public bool isActive;

    public PowderKegBuff(float duration, BuffManager manager, string source, float activationTime) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        this.activationTime = activationTime;
        manager.simulationManager.ShowText($"{manager.stats.name} has a Powder Keg for {duration} seconds!");
    }

    public override void Update()
    {
        if (!isActive && duration <= activationTime) isActive = true;

        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} removed Powder Keg!");
        manager.buffs.Remove("PowderKeg");
    }
}