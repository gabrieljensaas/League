public class RiposteBuff : Buff
{
    public RiposteBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} is Riposting for {duration} seconds!");
    }

    public override void Update()
    {
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name}'s Riposte Ended!");
        manager.buffs.Remove("Riposte");
    }
}