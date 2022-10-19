using UnityEngine;

public class AttackSpeedBuff : Buff
{
    public string uniqueKey;
    public AttackSpeedBuff(float duration, BuffManager manager, string source, float attackSpeed, string uniqueKey) : base(manager)
    {
        manager.combat.attackCooldown *= manager.stats.attackSpeed / (manager.stats.attackSpeed + (attackSpeed * manager.stats.attackSpeed));
        manager.stats.attackSpeed += attackSpeed * manager.stats.attackSpeed;
        this.value = attackSpeed;
        base.duration = duration;
        base.source = source;
        this.uniqueKey = uniqueKey;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {attackSpeed * manager.stats.attackSpeed:F3} Attack Speed from {source} for {duration:F3} Seconds!");
    }

    public override void Update()
    {
        if (!paused) duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value * manager.stats.attackSpeed} Extra Attack Speed From {source}!");
        manager.combat.attackCooldown *= manager.stats.attackSpeed / (manager.stats.attackSpeed - (value * manager.stats.attackSpeed));
        manager.stats.attackSpeed -= value * manager.stats.attackSpeed;
        manager.buffs.Remove(uniqueKey);
    }

    public void KillSilent()
    {
        manager.combat.attackCooldown *= manager.stats.attackSpeed / (manager.stats.attackSpeed - (value * manager.stats.attackSpeed));
        manager.stats.attackSpeed -= value * manager.stats.attackSpeed;
        manager.buffs.Remove(uniqueKey);
    }
}