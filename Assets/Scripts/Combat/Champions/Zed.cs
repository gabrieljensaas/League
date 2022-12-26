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

    public static float[] RExplodingDamageBonusBySkillLevel = { 0.25f, 0.4f, 0.55f };

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
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checkTakeDamage.Add(new CheckForHijackedZedR(this, this));

        qKeys.Add("Physical Damage");
        eKeys.Add("Physical Damage");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateTotalDamage(ref qSum, 0, new Damage(0, SkillDamageType.Phyiscal, (SkillComponentTypes)2048), QSkill().basic.name);
        for (int i = 0; i < shadowCount + 1; i++)
        {
            float damage = myStats.qSkill[0].UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats);
            UpdateTotalDamage(ref qSum, 0, new Damage(damage, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)16516), myStats.qSkill[0].basic.name);
            if (markedForDeath) markedRawDamage += damage;
        }
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, (SkillComponentTypes)2048), WSkill().basic.name);
        StartCoroutine(Shadow(5.25f));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, (SkillComponentTypes)2048), ESkill().basic.name);
        for (int i = 0; i < shadowCount + 1; i++)
        {
            float damage = myStats.eSkill[0].UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats);
            UpdateTotalDamage(ref eSum, 2, new Damage(damage, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)16512), myStats.eSkill[0].basic.name);
            myStats.wCD -= 2f;
            if (markedForDeath) markedRawDamage += damage;
        }
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        MyBuffManager.Add("Untargetable", new UntargetableBuff(0.95f, myStats.buffManager, myStats.rSkill[0].basic.name));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
        if (UpdateTotalDamage(ref rSum, 3, new Damage(0, SkillDamageType.Phyiscal, (SkillComponentTypes)2178), RSkill().basic.name) != float.MinValue)
        {
            yield return new WaitForSeconds(0.95f);
            StartCoroutine(MarkedForDeath());
        }
        else yield return new WaitForSeconds(0.95f);
        StartCoroutine(Shadow(9f));
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        var damage = UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Zed's Auto Attack");
        if (damage > 0 && markedForDeath) markedRawDamage += damage;
        if (damage != float.MinValue)
        {
            if (!usedPassive && targetStats.currentHealth <= targetStats.maxHealth * 0.5f)
            {
                var passive = UpdateTotalDamage(ref pSum, 4, new Damage(GetZedPassivePercentByLevel(myStats.level) * targetStats.maxHealth * 0.01f, SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)32), "Contempt for the Weak");
                StartCoroutine(ContempForTheWeak());
                simulationManager.AddCastLog(myCastLog, 4);
                if (passive > 0 && markedForDeath) markedRawDamage += passive;
            }
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
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
        UpdateTotalDamage(ref rSum, 3, new Damage(myStats.rSkill[0].UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) + (markedRawDamage * RExplodingDamageBonusBySkillLevel[myStats.rLevel]), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)32768), myStats.rSkill[0].basic.name);
    }

    public IEnumerator HMarkedForDeath(int skillLevel)
    {
        hMarkedForDeath = true;
        yield return new WaitForSeconds(3f);
        hMarkedForDeath = false;
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, new Damage(myStats.rSkill[0].UseSkill(skillLevel, rKeys[0], myStats, targetStats) + (hMarkedRawDamage * (markedRawDamage * RExplodingDamageBonusBySkillLevel[skillLevel])), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)32768), myStats.rSkill[0].basic.name);
    }
}