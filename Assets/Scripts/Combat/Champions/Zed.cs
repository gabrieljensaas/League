using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Zed : ChampionCombat
{
    public static float GetZedPassivePercentByLevel(int level)
    {
        return level switch
        {
            < 7 => 6,
            < 17 => 8,
            _ => 10
        };
    }

    public bool usedPassive = false;
    public int shadowCount = 0;
    public bool markedForDeath = false;
    public bool hMarkedForDeath = false;
    public float markedRawDamage = 0;
    public float hMarkedRawDamage = 0;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "Q", "E", "A" };

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
        checksW.Add(new CheckIfImmobilize(this));
        checksR.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        autoattackcheck = new ZedAACheck(this, this);
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checkTakeDamageAA.Add(new CheckForHijackedZedR(this, this));
        checkTakeDamageAbility.Add(new CheckForHijackedZedR(this, this));

        qKeys.Add("Physical Damage");
        eKeys.Add("Physical Damage");
        rKeys.Add("");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        for (int i = 0; i < shadowCount + 1; i++)
        {
            float damage = myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats);
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(damage, SkillDamageType.Phyiscal), myStats.qSkill[0].basic.name);
            if (markedForDeath) markedRawDamage += damage;
        }
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        StartCoroutine(Shadow(5.25f));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        for (int i = 0; i < shadowCount + 1; i++)
        {
            float damage = myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats);
            UpdateAbilityTotalDamage(ref eSum, 2, new Damage(damage, SkillDamageType.Phyiscal), myStats.eSkill[0].basic.name);
            myStats.wCD -= 2f;
            if (markedForDeath) markedRawDamage += damage;
        }
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(0.95f, myStats.buffManager, myStats.rSkill[0].basic.name));
        StartCoroutine(Shadow(9f));
        StartCoroutine(MarkedForDeath());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        if (targetStats.buffManager.HasImmobilize) yield break;
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(0.95f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        StartCoroutine(HMarkedForDeath(skillLevel));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
    }

    public IEnumerator ContempForTheWeak()
    {
        usedPassive = true;
        yield return new WaitForSeconds(10f);
        usedPassive = false;
    }

    public IEnumerator Shadow(float time)
    {
        shadowCount++;
        yield return new WaitForSeconds(time);
        shadowCount--;
    }

    public IEnumerator MarkedForDeath()
    {
        markedForDeath = true;
        yield return new WaitForSeconds(3f);
        markedForDeath = false;
        //exploding damage      0.55f needs to change by skill level
        UpdateAbilityTotalDamage(ref rSum, 3, new Damage(myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats) + (markedRawDamage * 0.55f), SkillDamageType.Phyiscal), myStats.rSkill[0].basic.name);
    }

    public IEnumerator HMarkedForDeath(int skillLevel)
    {
        hMarkedForDeath = true;
        yield return new WaitForSeconds(3f);
        hMarkedForDeath = false;
        //exploding damage      0.55f needs to change by skill level
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, new Damage(myStats.rSkill[0].UseSkill(skillLevel, rKeys[0], myStats, targetStats) + (hMarkedRawDamage * 0.55f), SkillDamageType.Phyiscal), myStats.rSkill[0].basic.name);
    }
}