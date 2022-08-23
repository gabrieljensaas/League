using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aatrox : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        base.UpdatePriorityAndChecks();
        combatPrio = new string[] { "R", "Q", "W", "A"};
        checksQ.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        checksA.Add(new CheckACD(this));
        autoattackcheck = new AatroxAACheck(this);
    }

    protected override void CheckPassive()
    {
        base.CheckPassive();

        if (!myStats.buffManager.buffs.ContainsKey("DeathbringerStance"))
        {
            myStats.buffManager.buffs.Add("DeathbringerStance", new DeathbringerStanceBuff(float.MaxValue, myStats.buffManager, myStats.passiveSkill.name));
        }
    }
}
