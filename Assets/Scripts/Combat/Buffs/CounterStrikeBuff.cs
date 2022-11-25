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
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        float minimumdamage = manager.combat.ESkill().UseSkill(manager.combat.myStats.eLevel, manager.combat.ESkill().name, manager.combat.myStats, manager.combat.targetStats);
        manager.combat.TargetBuffManager.Add("Stun", new StunBuff(1, manager.combat.TargetBuffManager, source));
        manager.combat.UpdateTotalDamage(ref manager.combat.eSum, 2, new Damage( minimumdamage + ((1 + (value * 0.2f)) * minimumdamage) + (manager.combat.myStats.bonusAD * 0.5f), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)18432), manager.combat.ESkill().name);

        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Counter Strike from {source}!");
        manager.buffs.Remove("CounterStrikeBuff");
    }
}
