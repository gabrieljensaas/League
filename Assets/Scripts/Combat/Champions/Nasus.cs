using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Nasus : ChampionCombat
{
    public int qStack = 0;        // nasus q has unlimited stack size, we will adjust this value by rounding it with game time
    public static float NasusPassiveLifeSteal(int level)
    {
        return level switch
        {
            < 7 => 0.09f,
            < 13 => 0.14f,
            _ => 0.19f
        };
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "Q", "A" };

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
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        autoattackcheck = new NasusAACheck(this);

        qKeys.Add("Bonus Physical Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage Per Tick");
        eKeys.Add("Armor Reduction");
        rKeys.Add("Bonus Health");
        rKeys.Add("Bonus Resistances");
        rKeys.Add("Total Magic Damage");

        myStats.lifesteal += NasusPassiveLifeSteal(myStats.level);
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("SiphoningStrike", new SiphoningStrikeBuff(10f, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats) + qStack));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        attackCooldown = 0;
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        targetStats.buffManager.buffs.Add("ArmorReduction", new ArmorReductionBuff(6f, targetStats.buffManager, myStats.eSkill[0].basic.name, myStats.eSkill[0].UseSkill(4, eKeys[2], myStats, targetStats), "ArmorReduction"));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        StartCoroutine(SpiritFire());
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("ArmorBuff", new ArmorBuff(rSum, myStats.buffManager, myStats.rSkill[0].basic.name, myStats.rSkill[0].UseSkill(2, rKeys[1], myStats, targetStats), "ArmorBuff"));
        myStats.buffManager.buffs.Add("MRBuff", new MagicResistanceBuff(rSum, myStats.buffManager, myStats.rSkill[0].basic.name,(int) myStats.rSkill[0].UseSkill(2, rKeys[1], myStats, targetStats), "MRBuff"));
        StartCoroutine(FuryOfTheSands(0));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public IEnumerator SpiritFire()
    {
        yield return new WaitForSeconds(0.25f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
    }

    public IEnumerator FuryOfTheSands(float time)
    {
        if (time == 0)
        {
            myStats.maxHealth += myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
            myStats.currentHealth += myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
            myStats.bonusHP += myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
            myStats.qSkill[0].basic.coolDown[2] *= 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            UpdateAbilityTotalDamage(ref rSum, 2, myStats.rSkill[0], 4, rKeys[2]);
        }

        if (time != 15f)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(FuryOfTheSands(time + 0.5f));
        }
        else
        {
            myStats.maxHealth -= myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
            myStats.currentHealth -= myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
            myStats.bonusHP -= myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
            myStats.qSkill[0].basic.coolDown[2] *= 2f;
        }
    }
}