using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Yorick : ChampionCombat
{
    public static float MistWalkerHP(int level) => 104 + (6 * level);
    public static float MistWalkerAA(int level)
    {
        return level switch
        {
            < 7 => 1 + 1 * level,
            < 13 => 8 + 5 * (level - 7),
            _ => 38 + 10 * (level - 13)
        };
    }

    public static float MistWalkerAS(int level) => 0.46f + (0.04f * level);

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "W", "A" };

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

        qKeys.Add("Bonus Physical Damage");
        wKeys.Add("Wall Health");
        eKeys.Add("Minimum Magic Damage");
        rKeys.Add("Mist Walkers");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        yield return base.ExecuteA();
        TouchOfTheMaidenProc();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        AutoAttack();
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        TouchOfTheMaidenProc();
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        yield return new WaitForSeconds(0.75f);
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.25f, targetStats.buffManager, myStats.wSkill[0].name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        float damage = (targetStats.maxHealth * 0.15f > myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats)) ? targetStats.maxHealth : myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats);
        UpdateAbilityTotalDamage(ref eSum, 2, damage, myStats.eSkill[0].name, SkillDamageType.Spell);
        targetStats.buffManager.buffs.Add("MourningMistCurse", new MourningMistCursedBuff(4, targetStats.buffManager, myStats.eSkill[0].name));

        foreach (Pet pet in pets.Where(pet => pet is MistWalker))
        {
            MistWalker mistWalker = pet as MistWalker;
            mistWalker.AutoAttack();
        }

        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        pets.Add(new MaidenOfTheMist(this, 3300 + (myStats.maxHealth * 0.75f), 40 + (myStats.AD * 0.5f), 1, 0, 0)); //all stats are for max level change when level adjusting of skills done
        for (int i = 0; i < myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats); i++)
            pets.Add(new MistWalker(this, MistWalkerHP(myStats.level) + (myStats.maxHealth * 0.2f), MistWalkerAA(myStats.level) + (myStats.AD * 0.25f), MistWalkerAS(myStats.level), 0 ,0));

        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    private void TouchOfTheMaidenProc()
    {
        if(targetStats.buffManager.buffs.TryGetValue("TouchOfTheMaiden", out Buff buff))
        {
            UpdateAbilityTotalDamage(ref rSum, 3, targetStats.maxHealth * 0.9f, myStats.rSkill[0].name, SkillDamageType.Spell);
            buff.Kill();
        }
    }
}
