using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Viego : ChampionCombat
{
    private bool wCast;
    private float timeSinceW;
    private float timeChanneled;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

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
        checksW.Add(new CheckIfImmobilize(this));
        checksR.Add(new CheckIfImmobilize(this));

        qKeys.Add("Bonus Physical Damage");
        qKeys.Add("Physical Damage");
        qKeys.Add("Minimum Bonus Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Bonus Attack Speed");
        rKeys.Add("Bonus Physical Damage");

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceW += Time.fixedDeltaTime;
        timeChanneled += Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[1], skillComponentTypes:(SkillComponentTypes)18560);
        BladeMark(QSkill().basic.name);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }


    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        if (!wCast)
        {
            yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
            timeSinceW = 0;
            timeChanneled = 0;
            wCast = true;
            myStats.wCD = 1;
            simulationManager.AddCastLog(myCastLog, 1);
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)34944);
            TargetBuffManager.Add("Stun", new StunBuff(0.25f + (timeChanneled > 1 ? 1 : timeChanneled), TargetBuffManager, "StunBuff")); //what happens when channeling gets cancelled
            BladeMark(WSkill().basic.name);
            myStats.wCD = WSkill().basic.coolDown[myStats.wLevel] - timeSinceW;
            simulationManager.AddCastLog(myCastLog, 1);
            wCast = false;
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        MyBuffManager.Add("AttackSpeed", new AttackSpeedBuff(8, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), "AttackSpeed"));
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;
        if (targetStats.PercentCurrentHealth < 0.5f) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0],skillComponentTypes: (SkillComponentTypes)34944);
        UpdateTotalDamage(ref rSum, 3, new Damage(myStats.AD * 1.2f * (1 + myStats.critStrikeChance), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)34944), RSkill().basic.name);
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        float procDamage = QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats) * 0.01f * targetStats.currentHealth;
        float markDamage = (0.2f * myStats.AD) + (myStats.AP * 0.15f);
        if (TargetBuffManager.buffs.TryGetValue("BladeOFRuinedKing", out Buff buff))
        {
            if(UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD + myStats.qLevel > -1 ? procDamage > QSkill().UseSkill(myStats.qLevel, qKeys[2], myStats, targetStats) ? procDamage : QSkill().UseSkill(myStats.qLevel, qKeys[2], myStats, targetStats) : 0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Viego's Auto Attack") != float.MinValue)
            {
                UpdateTotalDamage(ref qSum, 0, new Damage(markDamage, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)4136), QSkill().basic.name); // dont really know what i did here implementation might be incorrect.
            }
            buff.Kill();
            UpdateTotalHeal(ref qSum, markDamage * 1.35f, QSkill().basic.name);
        }
        else
        {
            UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD + myStats.qLevel > -1 ? procDamage > QSkill().UseSkill(myStats.qLevel, qKeys[2], myStats, targetStats) ? procDamage : QSkill().UseSkill(myStats.qLevel, qKeys[2], myStats, targetStats) : 0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Viego's Auto Attack");
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    private void BladeMark(string source)
    {
        if (targetStats.buffManager.buffs.TryGetValue("BladeOfRuinedKing", out Buff buff))
            buff.duration = 4;
        else
            TargetBuffManager.Add("BladeOfRuinedKing", new BladeOfRuinedKingBuff(4, TargetBuffManager, source));
    }
}