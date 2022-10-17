using UnityEngine;

public class NoxiousTrapBuff : Buff
{
    private float activationTime;
    public bool isActive;
    private float tickTimer = 0;

    public NoxiousTrapBuff(float duration, BuffManager manager, string source, float activationTime) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        this.activationTime = activationTime;
        manager.simulationManager.ShowText($"{manager.stats.name} has Poison for {duration} seconds from {source}!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();

        if (!isActive && duration <= activationTime) isActive = true;

        tickTimer += Time.fixedDeltaTime;
        if (tickTimer >= 1 && isActive)
        {
            manager.combat.targetCombat.UpdateAbilityTotalDamage(ref manager.combat.targetCombat.rSum, 2, manager.combat.targetStats.rSkill[1], 2, manager.combat.targetCombat.rKeys[0]);
            tickTimer = 0;
        }
    }

    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Poison from {source}!");
        manager.buffs.Remove("NoxiousTrap");
    }
}