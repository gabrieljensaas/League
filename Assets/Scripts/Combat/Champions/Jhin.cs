using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Jhin : ChampionCombat
{
    public static float GetJhinInnateBonusADPercent(int level)
    {
        //TODO: account for crit and attack speed from items when implemented
        return level switch
        {
            < 9 => 0.03f + 0.01f * level,
            < 11 => 0.12f + 0.02f * (level - 9),
            _ => 0.16f + 0.04f * (level - 11)
        };
    }

    public static float[] LotusRechargeBySkillLevel = { 24f, 21.5f, 19f, 16.5f, 14f };

    private float lotusTrapRecharge = 0;
    private int lotusTrapCharge = 2;
    private int whisperShot = 0;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "A", "W", "Q", "R" };

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

        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));

        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Minimum Physical Damage");
        wKeys.Add("Physical Damage");
        wKeys.Add("Root Duration");
        eKeys.Add("Magic Damage");
        rKeys.Add("Minimum Damage");

        base.UpdatePriorityAndChecks();
    }

    protected override void Start()
    {
        base.Start();
        myStats.AD += myStats.baseAD * GetJhinInnateBonusADPercent(myStats.level);
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        lotusTrapRecharge += Time.fixedDeltaTime;
        AddLotusTrapCharge();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (whisperShot == 4)
        {
            whisperShot = 0;
            UpdateTotalDamage(ref aSum, 5, new Damage((myStats.AD * 1.75f) + ((targetCombat.myStats.maxHealth - targetCombat.myStats.currentHealth) * 0.25f), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)1820), "Jhin's Auto Attack");
        }
        else
        {
            whisperShot++;
            UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Jhin's Auto Attack");
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)18564);
        ApplyDeadlyFlourishMark(myStats.qSkill[0].basic.name);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        if(UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)34948) != float.MinValue)
        {
            if (targetStats.buffManager.buffs.TryGetValue("Deadly Flourish Mark", out Buff deadlyFlourishMark))
                targetStats.buffManager.buffs.Add("Root", new RootBuff(myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), targetStats.buffManager, myStats.wSkill[0].basic.name));
        }
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (lotusTrapCharge > 0)
        {
            if (!CheckForAbilityControl(checksE)) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            StartCoroutine(LotusTrap());
            UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), ESkill().basic.name);
            ApplyDeadlyFlourishMark(myStats.qSkill[0].basic.name);
            myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
            simulationManager.AddCastLog(myCastLog, 2);
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateTotalDamage(ref rSum, 3, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), RSkill().basic.name);
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(10, myStats.buffManager, myStats.rSkill[0].basic.name, "CurtainCall"));
        StartCoroutine(CurtainCall());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(10, targetStats.buffManager, myStats.rSkill[0].basic.name, "CurtainCall"));
        yield return StartCoroutine(HCurtainCall(skillLevel));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
    }

    private void ApplyDeadlyFlourishMark(string skillName)
    {
        if (myStats.wCD > 0) return;

        TargetBuffManager.Add("Deadly Flourish Mark", new DeadlyFlourishBuff(4, targetStats.buffManager, skillName));
    }

    private void AddLotusTrapCharge()
    {
        if (myStats.eLevel != -1 && lotusTrapCharge < 2 && lotusTrapRecharge >= LotusRechargeBySkillLevel[myStats.eLevel])
        {
            lotusTrapCharge++;
            lotusTrapRecharge= 0;
        }
    }

    private IEnumerator CurtainCall()
    {
        int shots = 4;
        while (shots > 0)
        {
            if (shots == 1)
                UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)39044, damageModifier: 1 + (targetStats.PercentMissingHealth * 3));
            else
                UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)34948, damageModifier: 1 + (targetStats.PercentMissingHealth * 3));

            shots--;
            yield return new WaitForSeconds(0.25f);
        }

        myStats.buffManager.buffs.Remove("Channeling");
    }

    private IEnumerator HCurtainCall(int skillLevel)
    {
        int shots = 4;
        while (shots > 0)
        {
            if (shots == 1)
                UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]); //TODO: change to crit when Recep pushes new stuff
            else
                UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);

            shots--;
            yield return new WaitForSeconds(0.25f);
        }

        myStats.buffManager.buffs.Remove("Channeling");
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }

    public IEnumerator LotusTrap()
    {
        yield return new WaitForSeconds(3);       //1 second for arming and 2 seconds trigger
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], myStats.eLevel, skillComponentTypes: (SkillComponentTypes)16512);
    }
}