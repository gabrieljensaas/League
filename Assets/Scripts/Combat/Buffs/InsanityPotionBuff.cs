using UnityEngine;

public class InsanityPotionBuff : Buff
{
    public InsanityPotionBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} for {duration} seconds because of {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
/*        manager.buffs.Add("InsanityPotionArmor", new ArmorBuff());
        manager.buffs.Add("InsanityPotionHPRegen", new HPRegenBuff());
        manager.buffs.Add("InsanityPotionMR", new MRBuff());*/

        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} removed Insanity Potion");
        manager.buffs.Remove("Insanity Potion");
    }
}
