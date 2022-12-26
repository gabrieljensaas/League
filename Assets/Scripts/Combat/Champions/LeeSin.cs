using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class LeeSin : ChampionCombat
{
    public bool qCast = false;
    public float timeSinceQ = 0f;
    public bool wCast = false;
    private float timeSinceW = 0f;
    public bool eCast = false;
    private float timeSinceE = 0f;
    private int flurryStack = 0;

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
        checksA.Add(new CheckIfDisarmed(this));
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Physical Damage");
        qKeys.Add("Minimum Physical Damage");
        wKeys.Add("Shield Strength");
        wKeys.Add("Bonus Drain");
        eKeys.Add("Magic Damage");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceQ += Time.fixedDeltaTime;
        timeSinceW += Time.fixedDeltaTime;
        timeSinceE += Time.fixedDeltaTime;
    }


    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        if (!qCast)
        {
            yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
            UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes:(SkillComponentTypes)34944);
            StartCoroutine(SonicWave());
            myStats.qCD = 0.4f;
            timeSinceQ = 0;
            Flurry();
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
            if (myStats.buffManager.HasImmobilize) yield break;
            float extraDamage = myStats.qSkill[5].UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats) * (1 + ((targetStats.maxHealth - targetStats.currentHealth) / targetStats.maxHealth));
            UpdateTotalDamage(ref qSum, 0, new Damage(extraDamage, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)34944), myStats.qSkill[5].basic.name);
            StopCoroutine(SonicWave());
            Flurry();
            myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel] - timeSinceQ;

        }
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        if (!wCast)
        {
            yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
            if (myStats.buffManager.HasImmobilize) yield break;
            UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), WSkill().basic.name);
            MyBuffManager.shields.Add("ShieldBuff", new ShieldBuff(2, MyBuffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), "ShieldBuff"));
            StartCoroutine(SafeGuard());
            myStats.wCD = 0.4f;
            timeSinceW = 0;
            Flurry();
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
            MyBuffManager.Add("LifeStealBuff", new LifeStealBuff(4, myStats.buffManager, myStats.wSkill[1].basic.name, myStats.wSkill[1].UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), "LifeStealBuff"));
            // Need to add SpellVamp Buff 
            StopCoroutine(SafeGuard());
            Flurry();
            myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel] - timeSinceW;
        }
        simulationManager.AddCastLog(myCastLog, 1);

    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        if (!eCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], skillComponentTypes:(SkillComponentTypes)18560);
            StartCoroutine(Tempest());
            myStats.eCD = 0.4f;
            timeSinceE = 0;
            Flurry();
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            //only Slows them.
            UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), ESkill().basic.name);
            StopCoroutine(Tempest());
            Flurry();
            myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel] - timeSinceE;
        }
        simulationManager.AddCastLog(myCastLog, 2);

    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes:(SkillComponentTypes)34944);
        TargetBuffManager.Add("Airborne", new AirborneBuff(1f, targetStats.buffManager, "AirBorneBuff"));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Lee Sin's Auto Attack");

        if (flurryStack == 1) myStats.buffManager.buffs["Flurry"].Kill();
        else if (flurryStack > 0)
        {
            flurryStack--;
            myStats.buffManager.buffs["Flurry"].value = flurryStack;
            myStats.buffManager.buffs["Flurry"].duration = 3;
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public IEnumerator SonicWave()
    {
        qCast = true;
        yield return new WaitForSeconds(3f);
        qCast = false;
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel] - timeSinceQ;
    }

    public IEnumerator SafeGuard()
    {
        wCast = true;
        yield return new WaitForSeconds(3f);
        wCast = false;
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel] - timeSinceW;
    }
    public IEnumerator Tempest()
    {
        eCast = true;
        yield return new WaitForSeconds(3f);
        eCast = false;
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel] - timeSinceE;
    }

    public void Flurry()
    {
        if (!myStats.buffManager.buffs.TryAdd("Flurry", new AttackSpeedBuff(3, myStats.buffManager, myStats.passiveSkill.skillName, 0.4f * myStats.baseAttackSpeed, "Flurry")))
        {
            myStats.buffManager.buffs["Flurry"].value = 2;
            myStats.buffManager.buffs["Flurry"].duration = 3;
        }
    }
}