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
        float damage = Constants.GetDariusHemorrhageByLevel(manager.combat.targetStats.level, (int)value);

        _hemorrhageTimer += Time.deltaTime;
        if(_hemorrhageTimer >= 1.25f)
        {
            _hemorrhageTimer = 0f;
            manager.combat.targetCombat.UpdateAbilityTotalDamage(ref manager.combat.pSum, 4, damage, $"Hemorrhage {value}", SkillDamageType.PhysAndSpell);
        }

        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer Hemorrhaging By {source}");
        manager.buffs.Remove("Hemorrhage");
    }
}
