using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Kaisa : ChampionCombat
{
    public static float GetKaisaECastTime(float bonusAS)
    {
        if (bonusAS > 100) return 0.6f;
        return 1.2f - (0.006f * bonusAS);
    }

    public static float GetKaisaPassiveDamageByLevel(int level, int plasmaStacks, float AP)
    {
        if (level < 3) return 5 + (1 * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if (level < 4) return 8 + (1 * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if (level < 6) return 8 + (3.75f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if (level < 8) return 11 + (3.75f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if (level < 9) return 11 + (6.5f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if (level < 11) return 14 + (6.5f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if (level < 12) return 17 + (6.5f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if (level < 14) return 17 + (9.25f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if (level < 16) return 20 + (9.25f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if (level < 17) return 20 + (9.25f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        return 23 + (12f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "Q", "A" };

        autoattackcheck = new KaisaAACheck(this, this);
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
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checksR.Add(new CheckIfEnemyHasPlasma(this));
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksR.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Total Evolved Single-Target Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Bonus Attack Speed");
        rKeys.Add("Shield Strength");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(0.4f);                                //missle travel time
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
    }
    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        if (targetStats.buffManager.buffs.TryGetValue("Plasma", out Buff value))
        {
            value.value += 3;
            if (value.value > 4)
            {
                DealPassiveDamage((targetStats.maxHealth - targetStats.currentHealth) / 100 * (15 + (5 * (myStats.AP % 100))));
                value.value -= 5;
                if (value.value <= 0) value.Kill();
            }
        }
        else
        {
            PlasmaBuff buff = new PlasmaBuff(4, targetStats.buffManager, "Kaisa's Passive");
            buff.value = 3;
            targetStats.buffManager.buffs.Add("Plasma", buff);
        }
        myStats.wCD *= 0.33f;
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        myStats.eSkill[0].basic.castTime = GetKaisaECastTime(myStats.bonusAS);

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(0.5f, myStats.buffManager, myStats.eSkill[0].basic.name));
        myStats.buffManager.buffs.Add(myStats.eSkill[0].basic.name, new AttackSpeedBuff(4, myStats.buffManager, myStats.eSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, eKeys[0], myStats, targetStats), myStats.eSkill[0].basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.shields.Add(myStats.rSkill[0].basic.name, new ShieldBuff(2, myStats.buffManager, myStats.rSkill[0].basic.name, myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), myStats.rSkill[0].basic.name));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        if (!myStats.buffManager.buffs.ContainsKey("Plasma")) yield break;
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.shields.Add(myStats.rSkill[0].basic.name, new ShieldBuff(2, targetStats.buffManager, myStats.rSkill[0].basic.name, myStats.rSkill[0].SylasUseSkill(skillLevel, rKeys[0], targetStats, myStats), myStats.rSkill[0].basic.name));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
    }

    public void DealPassiveDamage(float rawdamage)
    {
        UpdateAbilityTotalDamage(ref pSum, 4, new Damage(rawdamage, SkillDamageType.Spell), myStats.passiveSkill.skillName);
    }
}