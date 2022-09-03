using UnityEngine;

public class CripplingStrikeBuff : Buff
{
    public CripplingStrikeBuff(float duration, BuffManager manager, string source, float strikedamage) : base(manager)
    {
        this.value = strikedamage;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Cripping Strike For {duration} Seconds It Will Deal Extra {strikedamage} Damage!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Crippling Strike Ended!");
        manager.buffs.Remove("Crippling Strike");
    }
}