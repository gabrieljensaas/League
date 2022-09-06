using System.Linq;
using UnityEngine;

public class BladeworkBuff : Buff
{
    public BladeworkBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;

        manager.combat.myStats.attackSpeed *= 1.9f;
        manager.simulationManager.ShowText($"{manager.stats.name} has used Bladework for {duration} seconds it will slow and crit!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.combat.myStats.attackSpeed /= 1.9f;
        manager.simulationManager.ShowText($"{manager.stats.name}'s Bladework Ended!");
        manager.buffs.Remove("Bladework");
    }
}