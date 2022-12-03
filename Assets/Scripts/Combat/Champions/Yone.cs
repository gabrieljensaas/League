using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Yone : ChampionCombat
{
    public int qStack = 0;
    public float EDamage = 0;
    public bool InE = false;
    public bool IsAzakanaSword = false;
    public static float GetYoneQCastTime(float bonusAS)
    {
        return bonusAS switch
        {
            < 15 => 0.4f,
            < 30 => 0.364f,
            < 45 => 0.328f,
            < 60 => 0.292f,
            < 75 => 0.256f,
            < 90 => 0.22f,
            < 105 => 0.184f,
            < 111.11f => 0.148f,
            _ => 0.133f
        };
    }
    public static float GetYoneQCooldown(float bonusAS)
    {
        return bonusAS switch
        {
            < 15 => 4f,
            < 30 => 3.64f,
            < 45 => 3.28f,
            < 60 => 2.92f,
            < 75 => 2.56f,
            < 90 => 2.2f,
            < 105 => 1.84f,
            < 111.11f => 1.48f,
            _ => 1.33f
        };
    }

    public static float GetYoneWCooldown(float bonusAS)
    {
        return bonusAS switch
        {
            < 10.5f => 16f,
            < 21f => 15f,
            < 31.5f => 14f,
            < 42f => 13f,
            < 52.5f => 12f,
            < 63f => 11f,
            < 73.5f => 10f,
            < 84f => 9f,
            < 94.5f => 8,
            < 105f => 7,
            _ => 6
        };
    }
    public static float GetYoneWCastTime(float bonusAS)
    {
        return bonusAS switch
        {
            < 10.5f => 0.5f,
            < 21f => 0.47f,
            < 31.5f => 0.44f,
            < 42f => 0.41f,
            < 52.5f => 0.38f,
            < 63f => 0.34f,
            < 73.5f => 0.31f,
            < 84f => 0.28f,
            < 94.5f => 0.25f,
            < 105f => 0.22f,
            _ => 0.19f
        };
    }
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "R", "W", "Q", "A" };

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
        checksR.Add(new CheckIfImmobilize(this));
        targetCombat.checkTakeDamagePostMitigation.Add(new CheckForYoneE(targetCombat, this));
        checksQ.Add(new CheckIfUnableToAct(this));
        checksW.Add(new CheckIfUnableToAct(this));
        checksE.Add(new CheckIfUnableToAct(this));
        checksR.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Physical Damage");
        wKeys.Add("Physical Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Damage Stored");
        rKeys.Add("Physical Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;
        if (qStack == 2 && myStats.buffManager.HasImmobilize) yield break;
        yield return StartCoroutine(StartCastingAbility(GetYoneQCastTime(myStats.bonusAS)));  
        if (qStack != 2)
        {
            UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0],skillComponentTypes: (SkillComponentTypes)14128);
            StartCoroutine(QStack());
        }
        else
        {
            if(UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)14130) != float.MinValue)
                TargetBuffManager.Add("Airborne", new AirborneBuff(0.75f, targetStats.buffManager, myStats.qSkill[0].basic.name));
            qStack = 0;
            StopCoroutine(QStack());
            StopCoroutine(QStack());
        }
        myStats.qCD = GetYoneQCooldown(myStats.bonusAS);
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(GetYoneWCastTime(myStats.bonusAS)));
        if(UpdateTotalDamage(ref wSum, 1, new Damage(myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)18560), myStats.wSkill[0].basic.name) != float.MinValue)
            UpdateTotalDamage(ref wSum, 1, new Damage(myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)16384), myStats.wSkill[0].basic.name);
        myStats.buffManager.shields.Add(myStats.wSkill[0].basic.name, new ShieldBuff(1.5f, myStats.buffManager, myStats.wSkill[0].basic.name, 2 * (35 + (20 / 17 * (myStats.level - 1)) + (myStats.bonusAD * 0.55f)), myStats.wSkill[0].basic.name));
        myStats.wCD = GetYoneWCooldown(myStats.bonusAS);
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2050), ESkill().basic.name);
        EDamage = 0;
        StartCoroutine(SoulUnbound());
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        if(UpdateTotalDamage(ref rSum, 3, new Damage(0, SkillDamageType.Phyiscal, (SkillComponentTypes)2177), RSkill().basic.name) != float.MinValue)
        {
            TargetBuffManager.Add("Airborne", new AirborneBuff(1.05f, targetStats.buffManager, myStats.rSkill[0].basic.name));
            MyBuffManager.Add("UnableToAct", new UnableToActBuff(0.3f, myStats.buffManager, myStats.rSkill[0].basic.name));
            yield return new WaitForSeconds(0.3f);
            UpdateTotalDamage(ref rSum, 3, new Damage(myStats.rSkill[0].UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)16384), myStats.rSkill[0].basic.name);
            UpdateTotalDamage(ref rSum, 3, new Damage(myStats.rSkill[0].UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats), SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)16384), myStats.rSkill[0].basic.name);
        }
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (IsAzakanaSword)
        {
            if (UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD * 0.5f, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Yone's Auto Attack") != float.MinValue)
                UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD * 0.5f, SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)32), "Yone's Auto Attack");
            IsAzakanaSword= false;
        }
        else
        {
            UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Yone's Auto Attack");
            IsAzakanaSword = true;
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        if (targetStats.buffManager.HasImmobilize) yield break;
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Airborne", new AirborneBuff(1.05f, myStats.buffManager, myStats.rSkill[0].basic.name));
        targetStats.buffManager.buffs.Add("UnableToAct", new UnableToActBuff(0.3f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
        yield return new WaitForSeconds(0.3f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, new Damage(myStats.rSkill[0].UseSkill(skillLevel, rKeys[0], targetStats, myStats), SkillDamageType.Phyiscal), myStats.rSkill[0].basic.name);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, new Damage(myStats.rSkill[0].UseSkill(skillLevel, rKeys[1], targetStats, myStats), SkillDamageType.Spell), myStats.rSkill[0].basic.name);
    }

    public IEnumerator QStack()
    {
        qStack++;
        yield return new WaitForSeconds(6f);
        qStack = 0;
    }

    public IEnumerator SoulUnbound()
    {
        InE = true;
        yield return new WaitForSeconds(5f);
        InE = false;
        UpdateTotalDamage(ref eSum, 2, new Damage(EDamage, SkillDamageType.True, skillComponentType: (SkillComponentTypes)18434), "Soul Unbound");
        StartCoroutine(StartCastingAbility(0.25f));
    }
}