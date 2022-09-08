using UnityEngine;

public class FlurryBuff : Buff
{
    public FlurryBuff(float duration, BuffManager manager, string source, float flurrydamage) : base(manager)
    {
        this.value = flurrydamage;
        base.duration = duration;
        base.source = source;
        manager.buffs.Remove("AsheQ");
        manager.simulationManager.ShowText($"{manager.stats.name} Has Entered Flurry For {duration} Seconds It Will Deal Extra {flurrydamage} Damage!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Flurry Ended!");
        manager.buffs.Remove("Flurry");
    }
}