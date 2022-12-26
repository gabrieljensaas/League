using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Syndra : ChampionCombat
{
    public int TranscendentStack;
    public float SpheresOnTheGround = 0;
    public int QStack = 1;
    public int MaxQStack = 1;
    public float QRecharge = 0;
    
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "W", "E", "A" };

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
        checksR.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        checksR.Add(new CheckIfExecutes(this, "Syndra",ref TranscendentStack, ref SpheresOnTheGround));

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage per Sphere");

        if (TranscendentStack >= 120) //increase ability power by %15
            if (TranscendentStack >= 40) MaxQStack = 2;
        if(myStats.rLevel >= 0) //q gains ability haste

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        QRecharge += Time.fixedDeltaTime;

        if(QRecharge >= QSkill().basic.coolDown[myStats.qLevel] && QStack < MaxQStack)
        {
            QStack++;
            QRecharge= 0;
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1 || QStack <= 0) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), QSkill().basic.name);
        myStats.qCD = 1.25f;
        QRecharge = 0;
        yield return new WaitForSeconds(0.6f);
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)16512);
        StartCoroutine(PutSphereOnTheGround());
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        if(UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)18560) != float.MinValue && TranscendentStack >= 60)
        {
            UpdateTotalDamage(ref wSum, 1,
                new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * (0.15f + (0.015f * myStats.AP)), SkillDamageType.True, skillComponentType: (SkillComponentTypes)16384), WSkill().basic.name);
        }
        StopCoroutine(PutSphereOnTheGround());
        SpheresOnTheGround--;
        StartCoroutine(PutSphereOnTheGround());
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        if(UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)18560) != float.MinValue)
        {
            TargetBuffManager.Add("Airborne", new AirborneBuff(0.1f, TargetBuffManager, ESkill().basic.name));
            if(SpheresOnTheGround > 0) TargetBuffManager.Add("Stun", new StunBuff(1.25f, TargetBuffManager, ESkill().basic.name));
        }
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime + 0.264f));
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)34948);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)32900);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)32900);

        int maxSphere = 0;
        while(SpheresOnTheGround > 0)
        {
            StopCoroutine(PutSphereOnTheGround());
            UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)32900);
            SpheresOnTheGround -= 1;
            maxSphere++;
            if (maxSphere == 4) break;
        }
        for(int i = 0; i < 3 + maxSphere; i++)
        {
            StartCoroutine(PutSphereOnTheGround());
        }
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        if (TranscendentStack >= 100 && targetStats.PercentCurrentHealth <= 0.15f)
            UpdateTotalDamage(ref rSum, 3, new Damage(targetStats.currentHealth, SkillDamageType.True), RSkill().basic.name); //execute
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Syndra's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public IEnumerator PutSphereOnTheGround()
    {
        SpheresOnTheGround++;
        yield return new WaitForSeconds(6f);
        SpheresOnTheGround--;
    }
}