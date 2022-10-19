using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Jinx : ChampionCombat
{
    public static float GetJinxWCastTime(float bonusAS)
    {
        if (bonusAS <= 250) return 0.6f - (0.02f * (bonusAS % 25));
        return 0.4f;
    }

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
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));

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
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal, SkillComponentTypes.Projectile | SkillComponentTypes.OnHit | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable));
        if(myStats.qLevel > -1)
        {
            StopCoroutine(QStackExpired());
            if (qStack != 3) qStack++;
            ((AttackSpeedBuff)myStats.buffManager.buffs[myStats.qSkill[0].basic.name]).KillSilent();
            myStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new AttackSpeedBuff(2.5f, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats) * (qStack + 1) * 0.5f, myStats.qSkill[0].basic.name));
            StartCoroutine(QStackExpired());
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        myStats.wSkill[0].basic.castTime = GetJinxWCastTime(myStats.bonusAS);

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        yield return new WaitForSeconds(0.9f); // chompers landing and arming time
        targetStats.buffManager.buffs.TryAdd("Root", new RootBuff(1.5f, targetStats.buffManager, myStats.eSkill[0].basic.name));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
    }

    private IEnumerator QStackExpired()
    {
        yield return new WaitForSeconds(2.5f);
        qStack--;
        ((AttackSpeedBuff)myStats.buffManager.buffs[myStats.qSkill[0].basic.name]).KillSilent();
        if (qStack > 0) myStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new AttackSpeedBuff(2.5f, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats) * (qStack + 1) * 0.5f, myStats.qSkill[0].basic.name));
    }
}