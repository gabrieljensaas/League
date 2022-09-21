using UnityEngine;

public class IonianFervorBuff : Buff
{
    public IonianFervorBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.duration = duration;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} has Ionian Fervor by {source} for {base.duration:F3} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Ionian Fervor!");
        manager.buffs.Remove("IonianFervor");
    }
}