using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class LeBlanc : ChampionCombat
{
    public bool UsedMirrorImage = false;
    public bool enemyMarked = false;
    public bool enemyMarkedR = false;
    private string lastUsedSkill = "";
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

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
        checksW.Add(new CheckIfImmobilize(this));
        checkTakeDamagePostMitigation.Add(new CheckLeBlancPassive(this, this));
        checkTakeDamagePostMitigation.Add(new CheckLeBlancPassive(this, this));
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Enhanced Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Enhanced Damage");


        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;
        lastUsedSkill = "Q";
        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if (enemyMarked) ExplodeMark();
        if (enemyMarkedR) ExplodeMarkR();
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        StartCoroutine(MarkEnemy());
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;
        lastUsedSkill = "W";
        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        if (enemyMarked) ExplodeMark();
        if (enemyMarkedR) ExplodeMarkR();
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;
        lastUsedSkill = "E";
        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        if (enemyMarked) ExplodeMark();
        if (enemyMarkedR) ExplodeMarkR();
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(1.5f);
        if (enemyMarked) ExplodeMark();
        if (enemyMarkedR) ExplodeMarkR();
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
        targetStats.buffManager.buffs.Add("Root", new RootBuff(1.5f, targetStats.buffManager, myStats.eSkill[0].basic.name));
    }

    public override IEnumerator ExecuteR()
    {
        switch (lastUsedSkill)
        {
            case "Q":
                if (!CheckForAbilityControl(checksQ)) yield break;
                yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
                if (enemyMarked) ExplodeMark();
                if (enemyMarkedR) ExplodeMarkR();
                UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
                myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
                StartCoroutine(MarkEnemyR());
                break;
            case "W":
                if (!CheckForAbilityControl(checksW)) yield break;
                yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
                if (enemyMarked) ExplodeMark();
                if (enemyMarkedR) ExplodeMarkR();
                UpdateTotalDamage(ref rSum, 1, myStats.rSkill[0], 2, rKeys[2]);
                myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
                break;
            case "E":
                if (!CheckForAbilityControl(checksE)) yield break;
                yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
                if (enemyMarked) ExplodeMark();
                if (enemyMarkedR) ExplodeMarkR();
                UpdateTotalDamage(ref rSum, 2, myStats.rSkill[0], 2, rKeys[3]);
                myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
                yield return new WaitForSeconds(1.5f);
                if (enemyMarked) ExplodeMark();
                if (enemyMarkedR) ExplodeMarkR();
                UpdateTotalDamage(ref rSum, 2, myStats.rSkill[0], 2, rKeys[4]);
                targetStats.buffManager.buffs.Add("Root", new RootBuff(1.5f, targetStats.buffManager, myStats.rSkill[0].basic.name));
                break;
            default:
                break;
        }
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        switch (lastUsedSkill)
        {
            case "Q":
                yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
                UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
                targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
                break;
            case "W":
                if (targetStats.buffManager.HasImmobilize) yield break;
                yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
                UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[2]);
                targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
                break;
            case "E":
                yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
                UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[3]);
                targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
                yield return new WaitForSeconds(1.5f);
                UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[3]);
                myStats.buffManager.buffs.Add("Root", new RootBuff(1.5f, myStats.buffManager, myStats.rSkill[0].basic.name));
                break;
            default:
                break;
        }
    }

    public IEnumerator MirrorImage()
    {
        UsedMirrorImage = true;
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(1f, myStats.buffManager, "Mirror Image"));
        yield return new WaitForSeconds(60f);
        UsedMirrorImage = false;
    }

    public IEnumerator MarkEnemy()
    {
        enemyMarked = true;
        yield return new WaitForSeconds(3.5f);
        enemyMarked = false;
    }
    public IEnumerator MarkEnemyR()
    {
        enemyMarkedR = true;
        yield return new WaitForSeconds(3.5f);
        enemyMarkedR = false;
    }

    public void ExplodeMark()
    {
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
    }

    public void ExplodeMarkR()
    {
        UpdateTotalDamage(ref rSum, 0, myStats.rSkill[0], 2, rKeys[1]);
    }
}