using System.Collections;
using UnityEngine;
using Simulator.Combat;
using System.Collections.Generic;

public class Twitch : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "W", "R", "A", "E" };

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

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Ambush", new AmbushBuff(14, targetStats.buffManager, myStats.wSkill[0].basic.name));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }
    public override IEnumerator ExecuteW()
    {
        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));

        if (myStats.buffManager.buffs.TryGetValue("Ambush", out Buff ambush))
            ambush.Kill();

        targetStats.buffManager.buffs.Add("Venom Cask", new VenomCaskBuff(3, targetStats.buffManager, myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));

        //TODO: Pass 35% Bonus AD to contaminate
        UpdateAbilityTotalDamage(ref eSum, 2, Constants.GetTwitchContaminateByLevel(myStats.level, (int)targetStats.buffManager.buffs["Deadly Venom"]?.value), myStats.eSkill[0].basic.name, SkillDamageType.Spell);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("SprayAndPray", new AttackDamageBuff(6, myStats.buffManager, myStats.rSkill[0].basic.name, 70, "SprayAndPray"));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));

        if(myStats.buffManager.buffs.TryGetValue("Ambush", out Buff ambush))
            ambush.Kill();

        AutoAttack();
        CheckDeadlyVenom("Auto Attack");
    }

    private void CheckDeadlyVenom(string skillName)
    {
        if (myStats.buffManager.buffs.TryGetValue("Deadly Venom", out Buff deadlyVenom))
        {
            if(deadlyVenom.value >= 6)
            {
                deadlyVenom.value = 6;
                deadlyVenom.duration = 6;
            }
            else
            {
                deadlyVenom.value++;
                deadlyVenom.duration = 6;
            }
        }
        else
        {
            targetStats.buffManager.buffs.Add("Deadly Venom", new DeadlyVenomBuff(6, targetStats.buffManager, skillName));
        }
    }
}