using Simulator.Combat;
using System.Collections.Generic;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    public ChampionStats stats;
    public ChampionCombat combat;
    public SimManager simulationManager;
    public List<Rune> runes = new();

    public RuneManager(ChampionStats stats, ChampionCombat combat, SimManager simManager)
    {
        this.stats = stats;
        this.combat = combat;
        simulationManager = simManager;
    }

    public void AddRune(Rune newRune)
    {
        runes.Add(newRune);
    }

    public void FixedUpdate()
    {
        foreach (Rune rune in runes)
            rune.Update();
    }
}