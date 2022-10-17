using UnityEngine;

public class SavageryBuff : Buff
{
    public SavageryBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        value = 2;
        manager.simulationManager.ShowText($"{manager.stats.name} has Savagery for {duration} seconds!");
        manager.Add("SavageryAS", new AttackSpeedBuff(duration, manager, source, 0.4f, "SavageryAS"));
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0 || value == 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Savagery!");
        manager.buffs.Remove("SavageryAS");
        manager.buffs.Remove("SavageryBuff");
    }
}