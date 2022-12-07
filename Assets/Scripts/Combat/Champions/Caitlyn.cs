using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Caitlyn : ChampionCombat
{
    public static float[] CaitlynTrapRechargeBySkillLevel = { 30, 24, 19, 15, 12 };

    public static float GetCaitlynPassivePercent(int level)
    {
        if (level < 7) return 60;
        if (level < 13) return 90;
        return 120;
    }

    private int wStack;
    private int wStackMax;
    private float wCD = 0;
    public bool enemyTrapped = false;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "W", "E", "A" };

        checksQ.Add(new CheckIfCasting(this));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfExecutes(this, "R"));

        qKeys.Add("Physical Damage");
        wKeys.Add("Maximum Traps");
        wKeys.Add("Headshot Damage Increase");
        eKeys.Add("Magic Damage");
        rKeys.Add("Physical damage");

        wStackMax = myStats.wLevel == -1 ? 0 : (int)myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats);
        wStack = wStackMax;
        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        wCD += Time.fixedDeltaTime;
        if (myStats.wLevel != -1 && wCD > CaitlynTrapRechargeBySkillLevel[myStats.wLevel])
        {
            if (wStack != wStackMax)
            {
                wStack++;
                wCD = 0f;
            }
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)18564);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;
        if (wStack <= 0) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        if(UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2176), WSkill().basic.name) != float.MinValue)
        {
            if (targetStats.buffManager.buffs.TryGetValue("YordleSnapTrap", out Buff value)) value.value += 1;
            else TargetBuffManager.Add("YordleSnapTrap", new YordleSnapTrapBuff(1, targetStats.buffManager, myStats.wSkill[0].basic.name));
        }
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        if(UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)34950) != float.MinValue)
        {
            MyBuffManager.Add("NetHeadshot", new NetHeadshotBuff(1.8f, myStats.buffManager, myStats.eSkill[0].basic.name));
        }
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateTotalDamage(ref rSum, 3, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), RSkill().basic.name);
        MyBuffManager.Add("Channeling", new ChannelingBuff(1, myStats.buffManager, myStats.rSkill[0].basic.name, "AceInTheHole"));
        StartCoroutine(AceInTheHole());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (myStats.buffManager.buffs.TryGetValue("NetHeadshot", out Buff v))
        {
            UpdateTotalDamage(ref aSum, 5, new Damage((GetCaitlynPassivePercent(myStats.level) + myStats.critStrikeChance) * myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Caitlyn's Auto Attack");
            v.Kill();
        }
        else if (myStats.buffManager.buffs.TryGetValue("TrapHeadshot", out Buff val))
        {
            UpdateTotalDamage(ref aSum, 5, new Damage(((GetCaitlynPassivePercent(myStats.level) + myStats.critStrikeChance) * myStats.AD) + myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Caitlyn's Auto Attack");
            val.Kill();
        }
        else if (myStats.buffManager.buffs.TryGetValue("Headshot", out Buff value))
        {
            if (value.value == 6)
            {
                UpdateTotalDamage(ref aSum, 5, new Damage((GetCaitlynPassivePercent(myStats.level) + myStats.critStrikeChance) * myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Caitlyn's Auto Attack");
                value.Kill();
            }
            else
            {
                value.value++;
            }
        }
        else UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Caitlyn's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(1, targetStats.buffManager, myStats.rSkill[0].basic.name, "HAceInTheHole"));
        StartCoroutine(HAceInTheHole(skillLevel));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
    }

    private IEnumerator AceInTheHole()
    {
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], 1 + myStats.critStrikeChance, skillComponentTypes: (SkillComponentTypes)32900);
    }

    private IEnumerator HAceInTheHole(int skillLevel)
    {
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0], 1 + 0);              //0 is critical chance fix it when items are added
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }
}