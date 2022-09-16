using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Akali : ChampionCombat
{
    public static float[] AkaliPassiveDamageByLevel = { 35, 38, 41, 44, 47, 50, 53, 62, 71, 8, 89, 98, 107, 122, 137, 152, 167, 182 };
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "Q", "W", "R", "A" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        autoattackcheck = new AkaliAACheck(this);
        checksA.Add(new CheckIfTotalCC(this));
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");


        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        yield return base.ExecuteQ();
        AssassinsMark();
    }

    public override IEnumerator ExecuteE()
    {
        yield return base.ExecuteE();
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }


    public void AssassinsMark()
	{
        if (!targetStats.buffManager.buffs.TryAdd("AkaliPassiveBuff", new AkaliPassiveBuff(4, targetStats.buffManager, myStats.passiveSkill.skillName)))
        {
            targetStats.buffManager.buffs["AkaliPassiveBuff"].duration = 4;
        }
    }
}