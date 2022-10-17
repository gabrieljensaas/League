using UnityEngine;

public class TumbleBuff : Buff
{
    public TumbleBuff(float duration, BuffManager manager, string source, float tumbledamage) : base(manager)
    {
        this.value = tumbledamage;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Tumbled For {duration} Seconds It Will Deal Extra {tumbledamage} Damage!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Tumble Ended!");
        manager.buffs.Remove("Tumble");
    }
}