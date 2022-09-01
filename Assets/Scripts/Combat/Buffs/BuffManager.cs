using Simulator.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// stats which states champion status and buffs/debuffs it has
/// </summary>
public class BuffManager
{
    public ChampionStats stats;
    public ChampionCombat combat;
    public SimManager simulationManager;
    public Dictionary<string,Buff> buffs = new Dictionary<string,Buff>();                                 //buffs and durations
    public Dictionary<string, ShieldBuff> shields = new Dictionary<string,ShieldBuff>();                                 //buffs and durations

    public BuffManager(ChampionStats stats, ChampionCombat combat, SimManager simManager)
    {
        this.stats = stats;
        this.combat = combat;
        simulationManager = simManager;
    }

    public void Update()                                      //check if any expired
    {
        foreach (var item in buffs.Values.ToList())
        {
            item.Update();
        }

        foreach (var item in shields.Values.ToList())
        {
            item.Update();
        }
    }

    public bool HasTotalCC
    {
        get
        {
            foreach (Buff buff in buffs.Values)
                if (buff is AirborneBuff 
                    or BerserkBuff 
                    or CharmBuff 
                    or FleeBuff
                    or TauntBuff 
                    or SleepBuff
                    or StasisBuff 
                    or StunBuff 
                    or SuppressionBuff 
                    or SuspensionBuff) return true;

            return false;
        }
    }

    public bool HasDisrupt
    {
        get
        {
            foreach (Buff buff in buffs.Values)
                if (buff is AirborneBuff 
                    or BerserkBuff 
                    or CharmBuff 
                    or FleeBuff
                    or TauntBuff 
                    or PolymorphBuff 
                    or SilenceBuff 
                    or SleepBuff 
                    or StasisBuff 
                    or StunBuff 
                    or SuppressionBuff 
                    or SuspensionBuff) 
                    return true;

            return false;
        }
    }

    public bool HasImmobilize
    {
        get
        {
            foreach (Buff buff in buffs.Values)
                if (buff is AirborneBuff
                    or BerserkBuff
                    or CharmBuff
                    or FleeBuff
                    or TauntBuff
                    or RootBuff
                    or SleepBuff
                    or StasisBuff
                    or StunBuff
                    or SuppressionBuff
                    or SuspensionBuff)
                    return true;

            return false;
        }
    }

    public bool HasDisarm
    {
        get
        {
            foreach (Buff buff in buffs.Values)
                if (buff is AirborneBuff
                    or CharmBuff
                    or DisarmBuff
                    or FleeBuff
                    or SleepBuff
                    or StasisBuff
                    or StunBuff
                    or SuppressionBuff
                    or SuspensionBuff)
                    return true;

            return false;
        }
    }
}