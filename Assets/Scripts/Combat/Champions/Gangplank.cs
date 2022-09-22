using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Gangplank : ChampionCombat
{
    private float trialByFireTimer = 0;
    private int powderKegCharges = 3; //hard coded for now
    private float powderKegTimer = 0;
    private float powderKegRechargeRate = 14; //hard coded for now

    public static float PowderKegActivationTime(int level)
    {
        return level switch
        {
            < 7 => 2,
            < 13 => 1,
            _ => 0.5f
        };
    }

    public static float TrialByFireBaseDamage(int level) => 40 + 15 * level;

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

        autoattackcheck = new JaxAACheck(this);
        checkTakeDamageAA.Add(new CheckCounterStrike(this));

        qKeys.Add("Physical Damage");
        wKeys.Add("Heal");
        eKeys.Add("Maximum charges");
        eKeys.Add("Slow");
        eKeys.Add("Champion Bonus Damage");
        rKeys.Add("Magic Damage Per Wave");
        rKeys.Add("Magic Damage Per Cluster");
        rKeys.Add("Total Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        powderKegTimer += Time.deltaTime;

        if (powderKegTimer > powderKegRechargeRate && powderKegCharges < (int)myStats.eSkill[0].UseSkill(2, eKeys[0], myStats, targetStats))
        {
            powderKegTimer = 0;
            powderKegCharges++;
        }

        trialByFireTimer -= Time.deltaTime;
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));

        if (myStats.buffManager.buffs.TryGetValue("PowderKeg", out Buff buff))
        {
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
            buff.Kill();
        }
        else
        {
            if(trialByFireTimer <= 0)
            {
                StartCoroutine(TrialByFire());
                trialByFireTimer = 15;
            }
            AutoAttack();
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys[0]);

        foreach(Buff buff in myStats.buffManager.buffs.Values)
            if (Buff.IsDisrupt(buff) && buff is not StasisBuff) buff.Kill();

        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;


        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));

        if (!myStats.buffManager.buffs.ContainsKey("PowderKeg") && powderKegCharges > 0)
            myStats.buffManager.buffs.Add("PowderKeg", new PowderKegBuff(25, myStats.buffManager, myStats.eSkill[0].name, PowderKegActivationTime(myStats.level)));

        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        StartCoroutine(CannonBarrage());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    //5 tick damage
    private IEnumerator TrialByFire()
    {
        for(int i = 0; i < 5; i++)
        {
            UpdateAbilityTotalDamage(ref pSum, 4, (TrialByFireBaseDamage(myStats.level) + myStats.bonusAD) / 5, myStats.passiveSkill.skillName, SkillDamageType.True);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator CannonBarrage()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[1], 2, rKeys[0]);
        }
    }
}
