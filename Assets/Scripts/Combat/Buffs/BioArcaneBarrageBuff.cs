using UnityEngine;

public class BioArcaneBarrageBuff : Buff
{
    public BioArcaneBarrageBuff(float duration, BuffManager manager, string source, float biodamage) : base(manager)
    {
        this.value = biodamage;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Entered Bio-Arcane Barrage For {duration} Seconds It Will Deal Extra {biodamage} Damage!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Bio-Arcane Barrage Ended!");
        manager.buffs.Remove("BioAArcaneBarrage");
    }
}