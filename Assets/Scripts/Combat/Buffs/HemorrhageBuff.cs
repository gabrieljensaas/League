using UnityEngine;

public class HemorrhageBuff : Buff
{
    private float _hemorrhageTimer = 1.25f;

    public HemorrhageBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Got Hemorrhage By {source} For {duration} Seconds!");
    }

    public override void Update()
    {
        float damage = Darius.GetDariusHemorrhageByLevel(manager.combat.targetStats.level, (int)value);

        _hemorrhageTimer += Time.fixedDeltaTime;
        if (_hemorrhageTimer >= 1.25f)
        {
            _hemorrhageTimer = 0f;
            manager.combat.targetCombat.UpdateTotalDamage(ref manager.combat.pSum, 4, new Damage(damage, SkillDamageType.Phyiscal,(SkillComponentTypes)67352), $"Hemorrhage {value}");
        }

        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Hemorrhaging By {source}");
        manager.buffs.Remove("Hemorrhage");
    }
}
