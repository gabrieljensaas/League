using UnityEngine;

public class YordleSnapTrapBuff : Buff
{
    public YordleSnapTrapBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        value = 1;
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Has a Yordle Snap Trap Underneath!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        if (value == 0)
        {
            manager.buffs.Remove("YordleSnapTrap");
        }
        else
        {
            value--;
            duration = 3f;
            manager.buffs.Add("Root", new RootBuff(1.5f, manager, manager.combat.targetStats.wSkill[0].basic.name));
            manager.combat.targetStats.buffManager.buffs.Add("TrapHeadshot", new TrapHeadshotBuff(1.8f, manager.combat.myStats.buffManager, manager.combat.targetStats.wSkill[0].basic.name));
        }
    }
}