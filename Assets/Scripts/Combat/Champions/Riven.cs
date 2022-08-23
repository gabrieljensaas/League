using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riven : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        base.UpdatePriorityAndChecks();
        combatPrio[0] = "A";
        combatPrio[1] = "Q";
        combatPrio[2] = "W";
        combatPrio[3] = "E";
        combatPrio[4] = "R";
    }
}
