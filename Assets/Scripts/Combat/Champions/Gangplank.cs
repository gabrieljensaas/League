using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gangplank : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "A", "R", "E", "W", "Q" };
        base.UpdatePriorityAndChecks();
    }
}
