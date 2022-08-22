using UnityEngine;

public class AsheQBuff : Buff
{
    public AsheQBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has {value} Ranger's Focus for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        if (value > 1)
        {
            value--;
            duration += 1;
            manager.simulationManager.ShowText($"{manager.stats.name} Lost a Ranger's Focus She Has {value} Now!");
        }
        else
        {
            manager.simulationManager.ShowText($"{manager.stats.name} Doesn't Have Any Ranger's Focus!");
            manager.buffs.Remove("AsheQ");
        }

    }
}