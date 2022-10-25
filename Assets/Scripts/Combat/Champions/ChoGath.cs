using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class ChoGath : ChampionCombat
{
    public int rStack = 0;
    private int eStack = 0;
    private float timeSinceE;
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
        checksR.Add(new CheckIfExecutes(this, "R"));

        qKeys.Add("Magic damage");
        wKeys.Add("Magic damage");
        wKeys.Add("Silence Duration");
        eKeys.Add("Magic Damage");
        rKeys.Add("Champion True Damage");

        rStack = 0;                               //change this to approximation later

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceE += Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(0.628f);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.buffManager.buffs.Add("Airborne", new AirborneBuff(1, targetStats.buffManager, myStats.qSkill[0].basic.name));
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.buffManager.buffs.Add("Silence", new SilenceBuff(myStats.wSkill[0].UseSkill(4, wKeys[1], myStats, targetStats), targetStats.buffManager, "Silence"));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;
        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        StartCoroutine(VorpalSpikes());
        myStats.eCD = 6;
        attackCooldown = 0;
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateTotalDamage(ref rSum, 3, new Damage(myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), myStats.rSkill[0].basic.name);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (eStack > 0)
        {
            UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
            eStack--;
            AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
            if (eStack == 0)
            {
                StopCoroutine(VorpalSpikes());
                eStack = 0;
                myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
            }
        }
        else
        {
            AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        }
    }

    public IEnumerator VorpalSpikes()
    {
        eStack = 3;
        yield return new WaitForSeconds(6f);
        eStack = 0;
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
    }
}