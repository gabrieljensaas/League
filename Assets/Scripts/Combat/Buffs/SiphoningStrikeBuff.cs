using UnityEngine;

public class SiphoningStrikeBuff : Buff
{
    public SiphoningStrikeBuff(float duration, BuffManager manager, string source, float strikedamage) : base(manager)
    {
        this.value = strikedamage;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Siphoning Strike For {duration} Seconds It Will Deal Extra {strikedamage} Damage!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Siphoning Strike Ended!");
        manager.buffs.Remove("SiphoningStrike");
    }
}