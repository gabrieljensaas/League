using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Jinx : ChampionCombat
{
    private int qStack = 0;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "A", "" };

        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfExecutes(this, "R"));

        qKeys.Add("Bonus Attack Speed");
        wKeys.Add("Physical Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Minimum Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        StopCoroutine(QStackExpired());
        AutoAttack();
        if (qStack != 3) qStack++;
        myStats.buffManager.buffs.Remove(myStats.qSkill[0].basic.name);
        myStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new AttackSpeedBuff(2.5f, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats) * (qStack + 1) * 0.5f, myStats.qSkill[0].basic.name));
        StartCoroutine(QStackExpired());
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        myStats.wSkill[0].basic.castTime = Constants.GetJinxWCastTime(myStats.bonusAS);

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        yield return new WaitForSeconds(0.9f); // chompers landing and arming time
        targetStats.buffManager.buffs.Add("Root", new RootBuff(1.5f, targetStats.buffManager, myStats.eSkill[0].basic.name));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    private IEnumerator QStackExpired()
    {
        yield return new WaitForSeconds(2.5f);
        qStack--;
        myStats.buffManager.buffs.Remove(myStats.qSkill[0].basic.name);
        if (qStack > 0) myStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new AttackSpeedBuff(2.5f, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats) * (qStack + 1) * 0.5f, myStats.qSkill[0].basic.name));
    }
}