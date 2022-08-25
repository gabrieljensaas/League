using System.Linq;
using UnityEngine;

public class WujuStyleBuff : Buff
{
    public WujuStyleBuff(float duration, BuffManager manager, string source, float bonusdamage) : base(manager)
    {
        this.value = bonusdamage;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Wuju Style On For {duration} Seconds It Will Deal Extra {bonusdamage} Damage!");
    }

    public override void Update()
    {
        if(!paused) duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Wuju Style Ended!");
        manager.buffs.Remove("WujuStyle");
    }
}