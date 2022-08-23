using Simulator.Combat;
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
}