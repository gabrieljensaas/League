using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Irelia : ChampionCombat
{
    public static float IonianFervorAttackSpeed(int level, int stack)
    {
        return level switch
        {
            < 7 => 7.5f * stack,
            < 13 => 13.75f * stack,
            _ => 20f * stack
        };
    }

    public static float IonianFervorEmpoweredDamage(int level) => 7 + (3 * level);

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "E", "W", "A" };

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

        autoattackcheck = new IreliaAACheck(this);

        qKeys.Add("Physical Damage");
        qKeys.Add("Heal");
        wKeys.Add("Minimum Physical Damage");
        wKeys.Add("Maximum Physical Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Cooldown Reduction");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        yield return base.ExecuteQ();
        IonianFervor(myStats.qSkill[0].name);

        if (targetStats.buffManager.buffs.TryGetValue("IonianFervorMark", out Buff buff))
        {
            myStats.qCD = 0.2f;
            buff.Kill();
        }
    }

    public override IEnumerator ExecuteW()
    {
        yield return base.ExecuteW(); //implementation is the instant W instead of hold W
        IonianFervor(myStats.wSkill[0].name);
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        yield return new WaitForSeconds(0.15f); //second E cast
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];

        targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.75f, targetStats.buffManager, myStats.eSkill[0].name));
        UnsteadyMark(myStats.eSkill[0].name);
        IonianFervor(myStats.eSkill[0].name);
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];

        UnsteadyMark(myStats.rSkill[0].name);
        IonianFervor(myStats.rSkill[0].name);
    }

    private void IonianFervor(string source)
    {
        if (myStats.buffManager.buffs.TryGetValue("IonianFervor", out Buff buff))
        {
            if (buff.value < 4)
                buff.value++;

            buff.duration = 6;
        }
        else
            myStats.buffManager.buffs.Add("IonianFervor", new IonianFervorBuff(6, myStats.buffManager, source));
    }

    private void UnsteadyMark(string source)
    {
        if (targetStats.buffManager.buffs.TryGetValue("IonianFervorMark", out Buff buff))
            buff.duration = 5;
        else
            targetStats.buffManager.buffs.Add("IonianFervorMark", new IonianFervorMarkBuff(6, targetStats.buffManager, source));
    }
}
