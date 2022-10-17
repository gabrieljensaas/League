using UnityEngine;

public class BerserkBuff : Buff
{
    public BerserkBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration * (100 - manager.stats.tenacity) / 100;
        base.source = source;

        if (manager.buffs.TryGetValue("Channeling", out Buff value))
            value.Kill();

        SimManager.Instance.AddBuffLog(new BuffLog(manager.stats.name, source, duration, SimManager.Instance.timer % 60, "Berserk"));
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Berserked by {source}!");
        manager.buffs.Remove("Berserk");
    }
}