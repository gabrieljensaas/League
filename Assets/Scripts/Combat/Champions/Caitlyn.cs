using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Caitlyn : ChampionCombat
{
    public static float[] CaitlynTrapRechargeBySkillLevel = { 30, 24, 19, 15, 12 };
    public static float[] CaitlynMaxTrapBySkillLevel = { 3, 3, 4, 4, 5 };

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
        autoattackcheck = new CaitlynAACheck(this);
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

        qKeys.Add("Physical Damage");
        wKeys.Add("Maximum Traps");
        wKeys.Add("Headshot Damage Increase");
        eKeys.Add("Magic Damage");
        rKeys.Add("Physical damage");

        wStackMax = (int)myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats);
        wStack = wStackMax;
        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        wCD += Time.deltaTime;
        if (wCD > CaitlynTrapRechargeBySkillLevel[4])
        {
            if (wStack != wStackMax)
            {
                wStack++;
                wCD = 0f;
            }
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;
        if (wStack <= 0) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        if (targetStats.buffManager.buffs.TryGetValue("YordleSnapTrap", out Buff value))
        {
            value.value += 1;
        }
        else
        {
            targetStats.buffManager.buffs.Add("YordleSnapTrap", new YordleSnapTrapBuff(1, targetStats.buffManager, myStats.wSkill[0].basic.name));
        }
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("NetHeadshot", new NetHeadshotBuff(1.8f, myStats.buffManager, myStats.eSkill[0].basic.name));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(1, myStats.buffManager, myStats.rSkill[0].basic.name, "AceInTheHole"));
        StartCoroutine(AceInTheHole());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
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
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0], 1 + 0);              //0 is critical chance fix it when items are added
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