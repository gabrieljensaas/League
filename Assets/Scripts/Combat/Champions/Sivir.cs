using Simulator.Combat;
using System.Collections;

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

        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));

        autoattackcheck = new SivirAACheck(this);
        checkTakeDamageAbility.Add(new CheckSpellShield(this));

        qKeys.Add("Champion Maximum Damage");
        wKeys.Add("Bonus Attack Speed");
        wKeys.Add("Bonus Attack Speed");
        rKeys.Add("Buff Duration");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteW()
    {
        myStats.buffManager.buffs.Add("Ricochet", new AttackSpeedBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), "Ricochet"));
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
        myStats.buffManager.buffs.Add("OnTheHunt", new OnTheHuntBuff(myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), myStats.buffManager, myStats.rSkill[0].basic.name));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        yield return null;
    }
}