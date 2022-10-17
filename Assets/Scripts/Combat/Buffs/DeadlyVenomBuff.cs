using UnityEngine;

public class DeadlyVenomBuff : Buff
{
    private float _poisonTimer = 1;

    public DeadlyVenomBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Got poisoned By {source} for {duration} seconds!");
    }

    public override void Update()
    {
        float damage = Twitch.GetTwitchDeadlyVenomByLevel(manager.combat.targetStats.level, (int)value); //TODO: add %ap damage

        _poisonTimer += Time.fixedDeltaTime;
        if (_poisonTimer >= 1f)
        {
            _poisonTimer = 0f;
            manager.combat.targetCombat.UpdateAbilityTotalDamage(ref manager.combat.targetCombat.pSum, 4, new Damage(damage, SkillDamageType.True), $"Deadly Venom {value}");
        }

        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer poisoned by {source}");
        manager.buffs.Remove("Deadly Venom");
    }
}
