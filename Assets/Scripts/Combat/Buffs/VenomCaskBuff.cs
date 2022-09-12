using UnityEngine;

public class VenomCaskBuff : Buff
{
    float timer;

    public VenomCaskBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Venom Cask for {duration} Seconds Because of {source}!");
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1)
        {
            if (manager.buffs.TryGetValue("Deadly Venom", out Buff deadlyVenom))
            {
                if (deadlyVenom.value >= 6)
                {
                    deadlyVenom.value = 6;
                    deadlyVenom.duration = 6;
                }
                else
                {
                    deadlyVenom.value++;
                    deadlyVenom.duration = 6;
                }
            }
            else
            {
                manager.buffs.Add("Deadly Venom", new DeadlyVenomBuff(6, manager, source));
            }
            timer = 0;
        }

        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} removed Venom Cask!");
        manager.buffs.Remove("Venom Cask");
    }
}