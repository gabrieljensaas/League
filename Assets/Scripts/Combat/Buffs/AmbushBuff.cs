using UnityEngine;

public class AmbushBuff : Buff
{
    public AmbushBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} camouflaged for {duration} seconds because of {source}!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        manager.buffs.Add("AmbushAS", new AttackSpeedBuff(5, manager, manager.combat.myStats.qSkill[0].basic.name, manager.combat.myStats.qSkill[0].UseSkill(4, manager.combat.qKeys[1], manager.stats, manager.combat.targetStats), "AmbushAS"));

        manager.simulationManager.ShowText($"{manager.stats.name} removed Ambush!");
        manager.buffs.Remove("Ambush");
    }
}
