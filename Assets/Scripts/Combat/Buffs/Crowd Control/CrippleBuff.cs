using UnityEngine;

public class CrippleBuff : Buff
{
    public CrippleBuff(float duration, BuffManager manager, string source, float value) : base(manager)
    {
        base.duration = duration * (100 - manager.stats.tenacity) / 100;
        base.source = source;
        base.value = value;

        manager.simulationManager.ShowText($"{manager.stats.name} is Crippled by {source} for {base.duration:F3} seconds!");
        manager.stats.attackSpeed *= 0.5f;
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.stats.attackSpeed /= 0.5f;

        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Crippled by {source}!");
        manager.buffs.Remove("Cripple");
    }
}