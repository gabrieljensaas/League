using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Soraka : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "Q", "R", "A", "W" };

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

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Magic Damage");
        qKeys.Add("Heal per Tick");

        eKeys.Add("Magic Damage");
        eKeys.Add("Root Duration");

        rKeys.Add("Heal");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)51204);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref qSum, RSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref qSum, RSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref qSum, RSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref qSum, RSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref qSum, RSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), RSkill().basic.name);
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref qSum, RSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), RSkill().basic.name);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)51200);
        TargetBuffManager.Add("Silence", new SilenceBuff(1.5f, TargetBuffManager, ESkill().basic.name));
        yield return new WaitForSeconds(1.5f);
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)51200);
        TargetBuffManager.Add("Root", new RootBuff(ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), TargetBuffManager, ESkill().basic.name));
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        if( myStats.PercentCurrentHealth < 0.4f)
		{
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * 0.5f, RSkill().basic.name);
        }
        UpdateTotalHeal(ref rSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

}