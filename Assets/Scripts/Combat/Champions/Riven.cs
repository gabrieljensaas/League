using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riven : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "A", "Q", "W", "E", "R" };
        base.UpdatePriorityAndChecks();
    }
}
