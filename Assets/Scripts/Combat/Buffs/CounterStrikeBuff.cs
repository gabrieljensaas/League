using UnityEngine;

public class CounterStrikeBuff : Buff
{
    public CounterStrikeBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} gained Counter Strike from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.combat.targetStats.buffManager.buffs.Add("Stun", new StunBuff(1, manager.combat.targetStats.buffManager, source));
        manager.combat.UpdateAbilityTotalDamage(ref manager.combat.eSum, 2, 155 + ((1 + (value * 0.2f)) * 155) + (manager.combat.myStats.bonusAD * 0.5f), manager.combat.myStats.eSkill[0].name, SkillDamageType.Phyiscal);

        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Counter Strike from {source}!");
        manager.buffs.Remove("CounterStrikeBuff");
    }
}
