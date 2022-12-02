using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Ekko : ChampionCombat
{
    public static float[] passiveDamageFlat = { 30, 40, 50, 60, 70, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140 };

    private float lostHealthPast4Seconds = 0;
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
        checksA.Add(new CheckIfDisarmed(this));
        checksE.Add(new CheckIfImmobilize(this));
        checksR.Add(new CheckIfImmobilize(this));
        checkTakeDamagePostMitigation.Add(new CheckForLostHealth(this, this));
        checkTakeDamagePostMitigation.Add(new CheckIfTargetable(this));
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Magic Damage");
        qKeys.Add("Magic Damage");
        wKeys.Add("Shield Strength");
        eKeys.Add("Bonus Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Heal");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if (UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)18564) != float.MinValue) AddZDriveResonance();
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
        yield return new WaitForSeconds(1.75f);
        if (UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)16516) != float.MinValue) AddZDriveResonance();
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), WSkill().basic.name);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        yield return new WaitForSeconds(3f);
        if (UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)128), WSkill().basic.name) != float.MinValue)
            TargetBuffManager.Add("Stun", new StunBuff(2.25f, TargetBuffManager, WSkill().basic.name));
        MyBuffManager.shields.Add(WSkill().basic.name, new ShieldBuff(2f, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), WSkill().basic.name));
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2050), ESkill().basic.name);
        MyBuffManager.Add(ESkill().basic.name, new PhaseDiveBuff(3, MyBuffManager, ESkill().basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        attackCooldown = 0;
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;
        if (myStats.PercentCurrentHealth > 0.5f) yield break;

        MyBuffManager.Add("Stasis", new StasisBuff(0.5f, MyBuffManager, RSkill().basic.name));
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateTotalHeal(ref hSum, RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats), RSkill().basic.name); //add health lost
        if (UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)18562) != float.MinValue) AddZDriveResonance();
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        if (MyBuffManager.buffs.TryGetValue("PhaseDive", out Buff value))
        {
            yield return StartCoroutine(StartCastingAbility(0.25f));

            if (UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5913), "Ekko's Auto Attack") != float.MinValue)
            {
                if (myStats.wLevel > -1 && targetStats.PercentMissingHealth <= 0.3f)
                {
                    float damage = (targetStats.maxHealth - targetStats.currentHealth) * (0.03f + (myStats.AP * 0.0003f));
                    UpdateTotalDamage(ref wSum, 1, new Damage(damage < 15 ? 15 : damage, SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)32), WSkill().basic.name);
                }

                if(UpdateTotalDamage(ref eSum, 2, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)32897), ESkill().basic.name) != float.MinValue)
                    AddZDriveResonance();
            }
            value.Kill();
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(0.1f));
            if (UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Ekko's Auto Attack") != float.MinValue && myStats.wLevel > -1 && targetStats.PercentMissingHealth <= 0.3f)
            {

                float damage = (targetStats.maxHealth - targetStats.currentHealth) * (0.03f + (myStats.AP * 0.0003f));
                UpdateTotalDamage(ref wSum, 1, new Damage(damage < 15 ? 15 : damage, SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)32), WSkill().basic.name);
                AddZDriveResonance();
            }
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public IEnumerator AddLostHealth(float hp)
    {
        lostHealthPast4Seconds += hp;
        yield return new WaitForSeconds(4f);
        lostHealthPast4Seconds -= hp;
    }

    public void AddZDriveResonance()
    {
        if (TargetBuffManager.buffs.TryGetValue("ZDriveResonance", out Buff value))
        {
            if (value.value == 0) return;
            value.value++;
            value.duration = 4;
        }
        else
        {
            TargetBuffManager.Add("ZDriveResonance", new ZDriveResonanceBuff(4, TargetBuffManager, "Z-Drive Resonance"));
        }
    }
}