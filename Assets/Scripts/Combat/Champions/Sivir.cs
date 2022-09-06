using System.Collections;
using UnityEngine;
using Simulator.Combat;

public class Sivir : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "R", "Q", "W", "A" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));

        checksQ.Add(new CheckIfCasting(this));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));

        autoattackcheck = new SivirAACheck(this);
        checkTakeDamageAbility.Add(new CheckSpellShield(this));

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteW()
    {
        myStats.buffManager.buffs.Add("Ricochet", new AttackSpeedBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].selfEffects.ASIncreasePercent[4], "Ricochet"));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        yield return null;
    }

    public override IEnumerator ExecuteE()
    {
        myStats.buffManager.buffs.Add("SpellShield", new SpellShieldBuff(1.5f, myStats.buffManager, myStats.eSkill[0].basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        yield return null;
    }

    public override IEnumerator ExecuteR()
    {
        myStats.buffManager.buffs.Add("OnTheHunt", new OnTheHuntBuff(12f, myStats.buffManager, myStats.rSkill[0].basic.name));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        yield return null;
    }
}