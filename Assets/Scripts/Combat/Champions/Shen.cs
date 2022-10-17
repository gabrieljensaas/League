using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Shen : ChampionCombat
{
    public float pCD = 0;
    public static float GetShenQBaseDamageByLevel(int level)
    {
        return level switch
        {
            < 4 => 10,
            < 7 => 16,
            < 10 => 22,
            < 13 => 28,
            < 16 => 34,
            _ => 40
        };
    }

    public static float GetShenPassiveCooldownByLevel(int level)
    {
        return level switch
        {
            < 6 => 6,
            < 9 => 5.5f,
            < 12 => 5,
            < 14 => 4.5f,
            < 16 => 4,
            < 17 => 3.5f,
            < 18 => 3,
            _ => 2.5f
        };
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        pCD -= Time.fixedDeltaTime;
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "A", "" };

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
        checksE.Add(new CheckIfImmobilize(this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Increased Bonus Damage");
        wKeys.Add("");
        eKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;
        GetPassiveShield();
        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("TwilightAssault", new TwilightAssaultBuff(8, myStats.buffManager, myStats.qSkill[0].basic.name, 3));
        myStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new AttackSpeedBuff(8, myStats.buffManager, myStats.qSkill[0].basic.name, 0.5f, myStats.qSkill[0].basic.name));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        //block basic attacks
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;
        GetPassiveShield();
        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        targetStats.buffManager.buffs.Add("Taunt", new TauntBuff(1.5f, targetStats.buffManager, myStats.eSkill[0].basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        yield break;
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield break;
    }

    public void GetPassiveShield()
    {
        if (pCD <= 0)
        {
            myStats.buffManager.shields.Add("Ki Barrier", new ShieldBuff(2.5f, myStats.buffManager, "Ki Barrier", 47 + (3 * myStats.level) + (myStats.bonusHP * 0.12f), "Ki Barrier"));
            pCD = GetShenPassiveCooldownByLevel(myStats.level);
        }
    }
}