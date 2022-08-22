using UnityEngine;

public class AttackSpeedBuff : Buff
{
    public string uniqueKey;
    public AttackSpeedBuff(float duration, BuffManager manager, string source, float attackSpeed, string uniqueKey) : base(manager)
    {
        manager.combat.AttackCooldown *= manager.stats.attackSpeed / (manager.stats.attackSpeed + attackSpeed);
        manager.stats.attackSpeed += attackSpeed;
        this.value = attackSpeed;
        base.duration = duration;
        base.source = source;
        this.uniqueKey = uniqueKey;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {attackSpeed:F3} Attack Speed from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if(duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value} Extra Attack Speed From {source}!");
        manager.combat.AttackCooldown *= manager.stats.attackSpeed / (manager.stats.attackSpeed - value);
        manager.stats.attackSpeed -= value;
        manager.buffs.Remove(uniqueKey);
    }
}