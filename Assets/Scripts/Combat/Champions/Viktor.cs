using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Viktor : ChampionCombat
{
    private bool qAugmented = true;
    private bool eAugmented = true;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "Q", "E", "A" };

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
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Modified Magic Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage Per Tick");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if (!qAugmented) myStats.buffManager.shields.Add(myStats.qSkill[0].basic.name, new ShieldBuff(2.5f, myStats.buffManager, myStats.qSkill[0].basic.name, 27 + (78 / 17 * (myStats.level - 1)) + (myStats.AP * 0.18f), myStats.qSkill[0].basic.name));
        else myStats.buffManager.shields.Add(myStats.qSkill[0].basic.name, new ShieldBuff(2.5f, myStats.buffManager, myStats.qSkill[0].basic.name, 40 + (8 * myStats.level) + (myStats.AP * 0.32f), myStats.qSkill[0].basic.name));
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)34948);
        MyBuffManager.Add("Discharge", new DischargeBuff(3.5f, myStats.buffManager, myStats.qSkill[0].basic.name));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), WSkill().basic.name);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        yield return new WaitForSeconds(2.25f);
        if (UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)128), WSkill().basic.name) != float.MinValue)
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(1.5f, targetStats.buffManager, myStats.wSkill[0].basic.name));
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)18564);
        if (eAugmented)
        {
            yield return new WaitForSeconds(1f);
            UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[1], skillComponentTypes: (SkillComponentTypes)16512);
        }
        simulationManager.AddCastLog(myCastLog,2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        if(UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)18560) != float.MinValue)
            TargetBuffManager.Add("Disrupt", new DisruptBuff(0, targetStats.buffManager, myStats.rSkill[0].basic.name));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        StartCoroutine(ChaosStorm());
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Disrupt", new DisruptBuff(0, myStats.buffManager, myStats.rSkill[0].basic.name));
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
        StartCoroutine(HChaosStorm(skillLevel));
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (myStats.buffManager.buffs.TryGetValue("Discharge", out Buff value))
        {
            UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)1816);
            value.Kill();
        }
        else UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Ahri's Auto Attack");

        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public IEnumerator ChaosStorm()
    {
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[1], skillComponentTypes: (SkillComponentTypes)16384);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[1], skillComponentTypes: (SkillComponentTypes)16384);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[1], skillComponentTypes: (SkillComponentTypes)16384);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[1], skillComponentTypes: (SkillComponentTypes)16384);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[1], skillComponentTypes: (SkillComponentTypes)16384);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[1], skillComponentTypes: (SkillComponentTypes)16384);
    }

    public IEnumerator HChaosStorm(int skillLevel)
    {
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
    }
}