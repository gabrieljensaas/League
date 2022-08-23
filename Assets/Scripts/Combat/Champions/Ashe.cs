using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulator.Combat;

public class Ashe : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        base.UpdatePriorityAndChecks();
        combatPrio[0] = "Q";
        combatPrio[1] = "A";
        combatPrio[2] = "W";
        combatPrio[3] = "R";
        checksQ.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckQCD(this));
        checksQ.Add(new CheckAsheQ(this));
        targetCombat.checksQ.Add(new CheckIfStunned(targetCombat));
        checksW.Add(new CheckIfCasting(this));
        checksW.Add(new CheckWCD(this));
        targetCombat.checksW.Add(new CheckIfStunned(targetCombat));
        checksE.Add(new CheckIfCasting(this));
        checksE.Add(new CheckECD(this));
        targetCombat.checksE.Add(new CheckIfStunned(targetCombat));
        checksR.Add(new CheckIfCasting(this));
        checksR.Add(new CheckRCD(this));
        targetCombat.checksR.Add(new CheckIfStunned(targetCombat));
        checksA.Add(new CheckIfCasting(this));
        checksA.Add(new CheckACD(this));
        targetCombat.checksA.Add(new CheckIfStunned(targetCombat));
        autoattackcheck = new AsheAACheck(this);
        targetCombat.checkTakeDamageAA.Add(new CheckIfFrosted(targetCombat));
        myUI.combatPriority.text = string.Join(", ", combatPrio);
    }
}
