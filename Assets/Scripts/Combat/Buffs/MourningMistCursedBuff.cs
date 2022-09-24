using UnityEngine;

public class MourningMistCursedBuff : Buff
{
    public MourningMistCursedBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        value = 8;
        manager.simulationManager.ShowText($"{manager.stats.name} has been Cursed from {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Curse from {source}!");
        manager.buffs.Remove("MourningMistCurse");
    }
}