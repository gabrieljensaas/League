using Newtonsoft.Json.Linq;
using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Varus : ChampionCombat
{
    public static float[] VarusWPassiveFlatBonusBySkillLevel = { 7, 12, 17, 22, 27 };

    public static float GetVarusWActiveTargetsMissingHealthMultiplier(int level)
    {
        return level switch
        {
            < 4 => 0.09f,
            < 7 => 0.12f,
            < 10 => 0.15f,
            < 13 => 0.18f,
            _ => 0.21f
        };
    }

    private bool qEmpowered;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "Q", "E", "A" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksW.Add(new CheckCD(this, "Q"));
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

        qKeys.Add("Maximum Physical Damage");
        wKeys.Add("Bonus Magic Damage");
        wKeys.Add("Bonus Magic Damage per Stack");
        eKeys.Add("Physical Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }
    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(1.25f, myStats.buffManager, myStats.qSkill[0].basic.name, "PiercingArrow"));
        UpdateTotalDamage(ref qSum, 0, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), QSkill().basic.name);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
        StartCoroutine(PiercingArrow());
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        qEmpowered = true;
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        yield return new WaitForSeconds(0.5f);
        if(UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)18564) != float.MinValue)
        {
            CheckBlightStacks();
        }
        TargetBuffManager.Add(myStats.eSkill[0].basic.name, new GrievousWoundsBuff(4, TargetBuffManager, myStats.eSkill[0].basic.name, 25f, myStats.eSkill[0].basic.name));
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        if(UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)18564) != float.MinValue)
        {
            CheckBlightStacks();
        }
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        TargetBuffManager.Add("Root", new RootBuff(2, TargetBuffManager, myStats.rSkill[0].basic.name));
        if (targetStats.buffManager.buffs.TryGetValue("Blight", out Buff value))
        {
            value.value = 3;
            value.duration = 6;
        }
        else
        {
            TargetBuffManager.Add("Blight", new BlightBuff(6, TargetBuffManager, "Chain of Corruption", 3));
        }
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
        myStats.buffManager.buffs.Add("Root", new RootBuff(2, myStats.buffManager, myStats.rSkill[0].basic.name));
        if (myStats.buffManager.buffs.TryGetValue("Blight", out Buff value))
        {
            value.value = 3;
            value.duration = 6;
        }
        else
        {
            myStats.buffManager.buffs.Add("Blight", new BlightBuff(6, myStats.buffManager, "Chain of Corruption", 3));
        }
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if(UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Ahri's Auto Attack") != float.MinValue)
        {
            UpdateTotalDamage(ref wSum, 1,
                new Damage(myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats),SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)32), WSkill().basic.name);
            if (targetStats.buffManager.buffs.TryGetValue("Blight", out Buff value))
            {
                if (value.value != 3)
                {
                    value.value++;
                }
                value.duration = 6;
            }
            else
            {
                TargetBuffManager.Add("Blight", new BlightBuff(6, TargetBuffManager, "Varus's Auto Attack"));
            }
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    private IEnumerator PiercingArrow()
    {
        yield return new WaitForSeconds(1.25f);
        if (UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)16516) != float.MinValue)
        {
            CheckBlightStacks(1.5f);
            if(qEmpowered)
            {
                UpdateTotalDamage(ref wSum, 1,
                    new Damage((targetStats.maxHealth - targetStats.currentHealth) * GetVarusWActiveTargetsMissingHealthMultiplier(myStats.level), SkillDamageType.Spell,
                    skillComponentType: (SkillComponentTypes)32), WSkill().basic.name);
                qEmpowered = false;
            }
        }
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }

    private void CheckBlightStacks(float multiplier = 1)
    {
        if (targetStats.buffManager.buffs.TryGetValue("Blight", out Buff value))
        {
            UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[1], multiplier * value.value, skillComponentTypes: (SkillComponentTypes)32);
            myStats.qCD -= value.value * myStats.qSkill[0].basic.coolDown[myStats.qLevel] * 0.12f;
            myStats.wCD -= value.value * myStats.wSkill[0].basic.coolDown[myStats.wLevel] * 0.12f;
            myStats.eCD -= value.value * myStats.eSkill[0].basic.coolDown[myStats.eLevel] * 0.12f;
            value.Kill();
        }
    }
}