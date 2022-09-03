using System.Collections;
using UnityEngine;
using Simulator.Combat;

public class Caitlyn : ChampionCombat
{
    private int wStack = (int)Constants.CaitlynMaxTrapBySkillLevel[4];
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

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        wCD += Time.deltaTime;
        if(wCD > Constants.CaitlynTrapRechargeBySkillLevel[4])
        {
            if (wStack != (int)Constants.CaitlynMaxTrapBySkillLevel[4])
            {
                wStack++;
                wCD = 0f;
            }
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;
        if(wStack <= 0) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        if(targetStats.buffManager.buffs.TryGetValue("YordleSnapTrap", out Buff value))
        {
            value.value += 1;
        }
        else
        {
            targetStats.buffManager.buffs.Add("YordleSnapTrap", new YordleSnapTrapBuff(1, targetStats.buffManager, myStats.wSkill[0].basic.name));
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;
        if (myStats.buffManager.HasImmobilize) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        myStats.buffManager.buffs.Add("NetHeadshot", new NetHeadshotBuff(1.8f, myStats.buffManager, myStats.eSkill[0].basic.name));
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(1, myStats.buffManager, myStats.rSkill[0].basic.name, "AceInTheHole"));
        StartCoroutine(AceInTheHole());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    private IEnumerator AceInTheHole()
    {
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, 1+ 0);              //0 is critical chance fix it when items are added
    }
}