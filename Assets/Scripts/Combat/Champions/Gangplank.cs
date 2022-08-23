using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gangplank : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        base.UpdatePriorityAndChecks();
        combatPrio[0] = "A";
        combatPrio[1] = "R";
        combatPrio[2] = "E";
        combatPrio[3] = "W";
        combatPrio[4] = "Q";
    }
}
