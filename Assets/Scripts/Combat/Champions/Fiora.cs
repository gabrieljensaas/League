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
        if (!targetStats.buffManager.buffs.TryGetValue("Vitals", out Buff _) && !targetStats.buffManager.buffs.TryGetValue("VitalsGrandChallenge", out Buff _))
        {
            VitalsBuff vitalsBuff = new(15, targetStats.buffManager, "Duelist's Dance")
            {
                activationTime = 13.25f
            };
            targetStats.buffManager.buffs.Add("Vitals", vitalsBuff);
        }
    }

    public override IEnumerator ExecuteA()
    {
        yield return StartCoroutine(base.ExecuteA());

        CheckVitals();

        yield return null;
    }

    public override IEnumerator ExecuteQ()
    {
        yield return StartCoroutine(base.ExecuteQ());

        CheckVitals();

        myStats.qCD *= 0.5f;
        yield return null;
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));

        myStats.buffManager.buffs.Add("Riposte", new RiposteBuff(0.75f, myStats.buffManager, myStats.wSkill[0].name));

        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];

        yield return null;
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));

        myStats.buffManager.buffs.Add("Bladework", new BladeworkBuff(4, myStats.buffManager, myStats.eSkill[0].name));

        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];

        yield return null;
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));

        if (targetStats.buffManager.buffs.TryGetValue("Vitals", out Buff vitals))
            vitals.Kill();

        VitalsBuff vitalsBuff = new(8, targetStats.buffManager, "Grand Challenge")
        {
            value = 4,
            activationTime = 7.5f
        };
        targetStats.buffManager.buffs.Add("VitalsGrandChallenge", vitalsBuff);

        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));

        VitalsBuff vitalsBuff = new(8, myStats.buffManager, "Grand Challenge")
        {
            value = 4,
            activationTime = 7.5f
        };
        myStats.buffManager.buffs.Add("VitalsGrandChallenge", vitalsBuff);

        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel];
    }

    private void CheckVitals()
    {
        if (targetStats.buffManager.buffs.TryGetValue("Vitals", out Buff vitals))
        {
            VitalsBuff vitalsBuff = (VitalsBuff)vitals;
            if (vitalsBuff.isActive)
            {
                UpdateAbilityTotalDamage(ref pSum, 4, new Damage(targetStats.maxHealth * 0.03f, SkillDamageType.True), "Vitals");
                vitalsBuff.Kill();
            }
        }
        else if (targetStats.buffManager.buffs.TryGetValue("VitalsGrandChallenge", out Buff vitalsUlt))
        {
            VitalsBuff vitalsBuff = (VitalsBuff)vitalsUlt;
            if (vitalsBuff.isActive)
            {
                UpdateAbilityTotalDamage(ref rSum, 3, new Damage(targetStats.maxHealth * 0.03f, SkillDamageType.True), "Vitals Grand Challenge");
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
            UpdateTotalHeal(ref hSum, myStats.rSkill[0].UseSkill(4, rKeys[0], myStats, targetStats), "Victory Zone"); //hardcoded healing
        }
    }
}
