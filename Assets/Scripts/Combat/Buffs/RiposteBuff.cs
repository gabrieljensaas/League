using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RiposteBuff : Buff
{
    public List<string> buffKeys = new();
    bool counterStun;

    public RiposteBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;

        foreach (string key in manager.buffs.Keys)
            buffKeys.Add(key);

        manager.simulationManager.ShowText($"{manager.stats.name} is Riposting for {duration} seconds!");
    }

    public override void Update()
    {
        if (!counterStun)
        {
            foreach (Buff buff in manager.buffs.Values)
                if (IsImmobilize(buff)) counterStun = true;
        }

        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }

    public override void Kill()
    {
        if (counterStun)
            manager.combat.targetStats.buffManager.buffs.Add("Stun", new StunBuff(2, manager.combat.targetStats.buffManager, manager.combat.myStats.wSkill[0].name));
        else
        {
            manager.combat.targetStats.buffManager.buffs.Add("Slow", new SlowBuff(2, manager.combat.targetStats.buffManager, manager.combat.myStats.wSkill[0].name));
            manager.combat.targetStats.buffManager.buffs.Add("Cripple", new CrippleBuff(2, manager.combat.targetStats.buffManager, manager.combat.myStats.wSkill[0].name, 0.5f));
        }

        //removing debuffs that were added during riposte
        foreach (string buff in manager.buffs.Keys.ToArray())
            if (!buffKeys.Contains(buff)) manager.buffs.Remove(buff);

        manager.simulationManager.ShowText($"{manager.stats.name}'s Riposte Ended!");
        manager.buffs.Remove("Riposte");
    }
}