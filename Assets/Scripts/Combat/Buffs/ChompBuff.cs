using UnityEngine;

public class ChompBuff : Buff
{
    public ChompBuff(float duration, BuffManager manager, string source, float strikedamage) : base(manager)
    {
        this.value = strikedamage;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Chomp For {duration} Seconds It Will Deal Extra {strikedamage} Damage!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Chomp Ended!");
        manager.buffs.Remove("Chomp");
    }
}