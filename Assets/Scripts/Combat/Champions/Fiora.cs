using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Fiora : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "E", "A", "W" };

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
        checksQ.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        autoattackcheck = new FioraAACheck(this);

        qKeys.Add("Physical Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Bonus Attack Speed");
        eKeys.Add("Critical damage");
        rKeys.Add("Heal per Tick");

        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }

    protected override void CheckPassive()
    {
        if (!TargetBuffManager.buffs.TryGetValue("Vitals", out Buff _) && !TargetBuffManager.buffs.TryGetValue("VitalsGrandChallenge", out Buff _))
        {
            VitalsBuff vitalsBuff = new(15, TargetBuffManager, "Duelist's Dance")
            {
                activationTime = 13.25f
            };
            TargetBuffManager.buffs.Add("Vitals", vitalsBuff);
        }
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Fiora's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
        CheckVitals();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)2970);
        myStats.qCD *= 0.5f;
        simulationManager.AddCastLog(myCastLog, 0);
        CheckVitals();

    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));

        MyBuffManager.Add("Riposte", new RiposteBuff(0.75f, MyBuffManager, myStats.wSkill[0].name));

        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)18560);
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);

    }

    public override IEnumerator ExecuteE()
    {

        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));

        MyBuffManager.Add("Bladework", new BladeworkBuff(4, MyBuffManager, myStats.eSkill[0].name));

        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];//2064 we are passing damage on autoattack checks
        simulationManager.AddCastLog(myCastLog, 2);

    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));

        if (TargetBuffManager.buffs.TryGetValue("Vitals", out Buff vitals))
            vitals.Kill();

        VitalsBuff vitalsBuff = new(8, TargetBuffManager, "Grand Challenge")
        {
            value = 4,
            activationTime = 7.5f
        };
        TargetBuffManager.Add("VitalsGrandChallenge", vitalsBuff);
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 4);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));

        VitalsBuff vitalsBuff = new(8, MyBuffManager, "Grand Challenge")
        {
            value = 4,
            activationTime = 7.5f
        };
        MyBuffManager.buffs.Add("VitalsGrandChallenge", vitalsBuff);

        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel];
    }

    private void CheckVitals()
    {
        if (TargetBuffManager.buffs.TryGetValue("Vitals", out Buff vitals))
        {
            VitalsBuff vitalsBuff = (VitalsBuff)vitals;
            if (vitalsBuff.isActive)
            {
                UpdateTotalDamage(ref pSum, 4, new Damage(targetStats.maxHealth * 0.03f, SkillDamageType.True, (SkillComponentTypes)1824), "Vitals");
                vitalsBuff.Kill();
            }
        }
        else if (TargetBuffManager.buffs.TryGetValue("VitalsGrandChallenge", out Buff vitalsUlt))
        {
            VitalsBuff vitalsBuff = (VitalsBuff)vitalsUlt;
            if (vitalsBuff.isActive)
            {
                UpdateTotalDamage(ref rSum, 3, new Damage(targetStats.maxHealth * 0.03f, SkillDamageType.True, (SkillComponentTypes)1824), "Vitals Grand Challenge");
                vitalsBuff.value--;

                if (vitalsBuff.value <= 0)
                {
                    vitalsBuff.Kill();
                    StartCoroutine(GrandChallengeHealing((int)(5 - vitalsBuff.value)));
                }
            }
        }
    }

    private IEnumerator GrandChallengeHealing(int vitalsHit)
    {
        for (int i = 0; i < vitalsHit; i++)
        {
            yield return new WaitForSeconds(1);
            UpdateTotalHeal(ref hSum, myStats.rSkill[0].UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), "Victory Zone"); //hardcoded healing
        }
    }
}
