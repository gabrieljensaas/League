using System.Linq;
using UnityEngine;

public class BladeworkBuff : Buff
{
    public BladeworkBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;

        manager.buffs.Add("BladeworkAS", new AttackSpeedBuff(duration, manager, source, manager.combat.myStats.eSkill[0].selfEffects.ASIncreasePercent[4], "BladeworkAS"));
        manager.simulationManager.ShowText($"{manager.stats.name} has used Bladework for {duration} seconds it will slow and crit!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Bladework Ended!");
        manager.buffs.Remove("Bladework");
    }
}