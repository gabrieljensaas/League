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
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Physical Damage Per Missile");
        qKeys.Add("Reduced Damage Per Missile");
        wKeys.Add("Magic Damage");
        eKeys.Add("Bonus Attack Speed");
        rKeys.Add("Shield Strength");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
        UpdateTotalDamage(ref qSum, 0, new Damage(0, SkillDamageType.Phyiscal, (SkillComponentTypes)2048), QSkill().basic.name);
        yield return new WaitForSeconds(0.4f);                                //missle travel time
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0],skillComponentTypes: (SkillComponentTypes)16516);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1],skillComponentTypes: (SkillComponentTypes)8324);
        yield return new WaitForSeconds(0.1f);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)8324);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)8324);
        yield return new WaitForSeconds(0.1f);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)8324);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)8324);
        yield return new WaitForSeconds(0.1f);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)8324);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)8324);
        yield return new WaitForSeconds(0.1f);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)8324);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)8324);
        yield return new WaitForSeconds(0.1f);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)8324);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)8324);
        simulationManager.AddCastLog(myCastLog, 0);
    }
    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)34948);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
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
            TargetBuffManager.Add("Plasma", buff);
        }
        myStats.wCD *= 0.33f;
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        myStats.eSkill[0].basic.castTime = GetKaisaECastTime(myStats.bonusAS);

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        MyBuffManager.Add("Untargetable", new UntargetableBuff(0.5f, myStats.buffManager, myStats.eSkill[0].basic.name));
        MyBuffManager.Add(myStats.eSkill[0].basic.name, new AttackSpeedBuff(4, myStats.buffManager, myStats.eSkill[0].basic.name,
            myStats.eSkill[0].UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), myStats.eSkill[0].basic.name));
        UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, buffNames:new string[] {"Untargetable", ESkill().basic.name },
            skillComponentType: (SkillComponentTypes)2048), ESkill().basic.name);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.shields.Add(myStats.rSkill[0].basic.name, new ShieldBuff(2, myStats.buffManager, myStats.rSkill[0].basic.name,
            myStats.rSkill[0].UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), myStats.rSkill[0].basic.name));
        UpdateTotalDamage(ref rSum, 3, 
            new Damage(0, SkillDamageType.Phyiscal, buffNames: new string[] {RSkill().basic.name}, skillComponentType: (SkillComponentTypes)2050), RSkill().basic.name);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if(UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Kai'Sa's Auto Attack") != float.MinValue)
        {
            if (TargetBuffManager.buffs.TryGetValue("Plasma", out Buff value))
            {
                UpdateTotalDamage(ref pSum, 4, 
                    new Damage(GetKaisaPassiveDamageByLevel(myStats.level, (int)value.value, myStats.AP), SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)36), "Second Skin");
                if (value.value == 5)
                {
                    DealPassiveDamage((targetStats.maxHealth - targetStats.currentHealth) / 100 * (15 + (5 * (myStats.AP % 100))));
                    simulationManager.AddCastLog(myCastLog, 4);
                    value.Kill();
                }
                else
                {
                    value.value++;
                    value.duration = 4f;
                }
            }
            else
            {
                TargetBuffManager.Add("Plasma", new PlasmaBuff(4, targetStats.buffManager, "Kai'Sa's Auto Attacks"));
            }
            myStats.eCD -= 0.5f;
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
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
        UpdateTotalDamage(ref pSum, 4, new Damage(rawdamage, SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)36), myStats.passiveSkill.skillName);
        simulationManager.AddCastLog(myCastLog, 4);
    }
}