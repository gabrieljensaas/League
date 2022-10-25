using UnityEngine;

public class PoisonTrailBuff : Buff
{
    private float _poisonTimer = 0f;

    public PoisonTrailBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Got poisoned By {source} for {duration} seconds!");
    }

    public override void Update()
    {
        _poisonTimer += Time.fixedDeltaTime;
        if (_poisonTimer >= 0.25f)
        {
            _poisonTimer = 0f;
            manager.combat.targetCombat.UpdateTotalDamage(ref manager.combat.targetCombat.qSum, 0, manager.combat.targetStats.qSkill[0], 4, manager.combat.targetCombat.qKeys[0]);
        }

        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} is no longer poisoned by {source}");
        manager.buffs.Remove("Poison Trail");
    }
}
