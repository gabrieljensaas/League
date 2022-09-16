using UnityEngine;

public class GritBuff : Buff
{
    private int tick = 0;
    private Sett sett;
    public GritBuff(float duration, BuffManager manager, string source, float value, Sett sett) : base(manager)
    {
        this.value = value;
        base.source = source;
        base.duration = duration;
        this.sett = sett;
        manager.simulationManager.ShowText($"{manager.stats.name} Has Gained {value} Grit From {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            switch (tick)
            {
                case 0:
                    sett.grit -= value * 0.3f;
                    manager.simulationManager.ShowText($"{manager.stats.name} Lost {value * 0.3f} Grit!");
                    value *= 0.7f;
                    duration = 1;
                    tick++;
                    break;
                case 1:
                    sett.grit -= value * 0.429f;
                    manager.simulationManager.ShowText($"{manager.stats.name} Lost {value * 0.429f} Grit!");
                    value *= 0.571f;
                    duration = 1;
                    tick++;
                    break;
                case 2:
                    sett.grit -= value * 0.75f;
                    manager.simulationManager.ShowText($"{manager.stats.name} Lost {value * 0.75f} Grit!");
                    value *= 0.25f;
                    duration = 1;
                    tick++;
                    break;
                default:
                    sett.grit -= value;
                    manager.simulationManager.ShowText($"{manager.stats.name} Lost {value} Grit!");
                    Kill();
                    break;
            }
        }
    }
    public override void Kill()
    {
        sett.gritList.Remove(this);
    }
}