using UnityEngine;

public class IonianFervorMarkBuff : Buff
{
    public IonianFervorMarkBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} has been marked by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has mark!");
        manager.buffs.Remove("IonianFervorMark");
    }
}
