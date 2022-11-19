using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sett : ChampionCombat
{
    public float pCD = 0;
    public float knuckleDown = 0;
    public bool leftPunched = false;
    public float grit = 0;
    public List<GritBuff> gritList = new();
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "W", "A" };

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
        checksR.Add(new CheckIfImmobilize(this));
        checkTakeDamagePostMitigation.Add(new CheckForGrit(this, this));

        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Bonus Physical Damage");
        wKeys.Add("Damage");
        eKeys.Add("Physical Damage");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        pCD += Time.fixedDeltaTime;
        if (pCD >= 2) leftPunched = false;
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        pCD = 0;
        leftPunched = false;
        StartCoroutine(KnuckleDown());
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        StartCoroutine(HaymakerShield());
        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateTotalDamage(ref wSum, 1, new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats, grit), SkillDamageType.True, (SkillComponentTypes)16512), WSkill().basic.name);
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        TargetBuffManager.Add("Airborne", new AirborneBuff(0.1f, TargetBuffManager, ESkill().basic.name));
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)18560);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
        yield return StartCoroutine(StartCastingAbility(0.25f));
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        TargetBuffManager.Add("Suppression", new SuppressionBuff(1.5f, TargetBuffManager, RSkill().basic.name));
        yield return StartCoroutine(StartCastingAbility(1.23f));
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes:(SkillComponentTypes)18562);
        yield return StartCoroutine(StartCastingAbility(0.27f));
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        if (targetStats.buffManager.HasImmobilize) yield break;
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
        myStats.buffManager.buffs.Add("Suppression", new SuppressionBuff(1.5f, myStats.buffManager, myStats.rSkill[0].basic.name));
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(1.23f));
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(0.27f));
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        if (knuckleDown > 0)
        {
            yield return StartCoroutine(StartCastingAbility(1f));
            UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD + QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Sett Empowered Q Auto Attack");
            pCD = 0;
            leftPunched = false;
            knuckleDown--;
        }
        else if (leftPunched)
        {
            yield return StartCoroutine(StartCastingAbility(0.0125f));
            UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD + (5 * myStats.level) + (myStats.bonusAD), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "SettEmpowered Auto Attack");
            pCD = 0;
            leftPunched = false;
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(0.1f));
            UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Sett's Auto Attack");
            leftPunched = true;
            pCD = 0;
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public IEnumerator KnuckleDown()
    {
        knuckleDown += 2;
        yield return new WaitForSeconds(5);
        knuckleDown = 0;
    }

    public IEnumerator HaymakerShield()
    {
        MyBuffManager.Add(myStats.wSkill[0].basic.name, new ShieldBuff(3, MyBuffManager, myStats.wSkill[0].basic.name, grit, WSkill().basic.name, decaying: true));
        yield return new WaitForSeconds(0.75f);
        if (MyBuffManager.shields.TryGetValue(WSkill().basic.name, out ShieldBuff value))
        {
            value.decaying = true;
        }
    }
}