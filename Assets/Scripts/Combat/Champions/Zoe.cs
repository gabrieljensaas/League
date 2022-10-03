using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Zoe : ChampionCombat
{
    public static float[] ZoeQBaseDamageByLevel = { 7, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 29, 32, 35, 38, 42, 46, 50 };
    public static float[] ZoePassiveDamageByLevel = { 16, 20, 24, 28, 32, 36, 42, 48, 54, 60, 66, 74, 82, 90, 100, 110, 120, 130 };
    public bool HasPassive = false;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "Q", "A", "", "" };

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
        checksR.Add(new CheckIfImmobilize(this));
        targetCombat.checkTakeDamageAAPostMitigation.Add(new CheckIfSleptByZoe(targetCombat));
        targetCombat.checkTakeDamageAbilityPostMitigation.Add(new CheckIfSleptByZoe(targetCombat));
        autoattackcheck = new ZoeAACheck(this, this);

        qKeys.Add("Minimum Magic Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("");
        eKeys.Add("");
        eKeys.Add("");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, new Damage(myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats) - ZoeQBaseDamageByLevel[4] + ZoeQBaseDamageByLevel[myStats.level], SkillDamageType.Spell), myStats.qSkill[0].basic.name);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(1.4f);
        targetStats.buffManager.buffs.Add("Sleep", new SleepBuff(2.25f, targetStats.buffManager, myStats.eSkill[0].basic.name));
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield break;
    }

    public IEnumerator ZoePassive()
    {
        HasPassive = true;
        yield return new WaitForSeconds(5f);
        HasPassive = false;
    }
}