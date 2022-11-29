using Simulator.Combat;
using System.Collections;
using UnityEngine;
/// <summary>
/// needs more in depth research for reload speed it is not much detailed in wiki
/// </summary>
public class Graves : ChampionCombat
{
    public static float[] BasicADPercentByLevel = { 0.7f, 0.7113f, 0.7234f, 0.7363f, 0.75f, 0.7644f, 0.7797f, 0.7958f, 0.8126f, 0.8303f, 0.8487f, 0.8679f, 0.888f, 0.9088f, 0.9304f,0.9528f, 0.976f, 1.01f };
    public static float[] BonusPelletADPercentByLevel = { 0.2331f, 0.2369f, 0.2409f, 0.2452f, 0.2497f, 0.2546f, 0.2597f, 0.265f, 0.2706f, 0.2765f, 0.2826f, 0.289f, 0.2957f, 0.3026f, 0.3098f, 0.3173f, 0.325f, 0.333f };
    public float timeSinceLastAA = 0f;
    public float reloadSpeed;
    public int currentAmmo = 2;
    public int trueGrit = 0;
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
        checksE.Add(new CheckIfImmobilize(this));
        checksR.Add(new CheckIfImmobilize(this));

        qKeys.Add("Physical Damage");
        qKeys.Add("Physical Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Bonus Armor");
        rKeys.Add("Physical Damage");

        reloadSpeed = myStats.bonusAS / myStats.baseAttackSpeed > 4 ? 1.12f : 2.08f - ((0.651f + (0.014f * myStats.level)) / 4f * (myStats.bonusAS / myStats.baseAttackSpeed));

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        timeSinceLastAA += Time.fixedDeltaTime;

        if (currentAmmo != 2)
        {
            if (currentAmmo == 1 && timeSinceLastAA >= reloadSpeed - 0.78f)
            {
                currentAmmo = 2;
                timeSinceLastAA = 0;
            }
            else if (timeSinceLastAA >= reloadSpeed)
            {
                currentAmmo = 2;
                timeSinceLastAA = 0;
            }
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;
        timeSinceLastAA = 0;
        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        timeSinceLastAA = 0;
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
        if(UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)18564) != float.MinValue)
        {
            yield return new WaitForSeconds(2f);
            UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)16516);
        }
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;
        timeSinceLastAA = 0;
        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        timeSinceLastAA = 0;
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)18454);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        timeSinceLastAA = 0;
        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        timeSinceLastAA = 0;
        if (currentAmmo != 2) currentAmmo++;
        UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2050), ESkill().basic.name);
        StopCoroutine(TrueGrit());
        myStats.armor -= trueGrit * ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats);
        trueGrit = trueGrit + 2 > 8 ? 8 : trueGrit + 2;
        StartCoroutine(TrueGrit());
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        attackCooldown = 0;
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;
        timeSinceLastAA = 0;
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        timeSinceLastAA = 0;
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)34950);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;
        if (currentAmmo == 0) yield break;
        timeSinceLastAA = 0;
        yield return StartCoroutine(StartCastingAbility(0.1f));
        timeSinceLastAA = 0;
        if (currentAmmo == 2)
        {
            UpdateTotalDamage(ref aSum, 5, new Damage(BasicADPercentByLevel[myStats.level - 1] * myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Graves's Auto Attack");
            UpdateTotalDamage(ref aSum, 5, new Damage(BonusPelletADPercentByLevel[myStats.level - 1] * myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)71428), "Graves's Pellet");
            UpdateTotalDamage(ref aSum, 5, new Damage(BonusPelletADPercentByLevel[myStats.level - 1] * myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)71428), "Graves's Pellet");
            UpdateTotalDamage(ref aSum, 5, new Damage(BonusPelletADPercentByLevel[myStats.level - 1] * myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)71428), "Graves's Pellet");
        }
        else if (currentAmmo == 1)
        {
            UpdateTotalDamage(ref aSum, 5, new Damage(BasicADPercentByLevel[myStats.level - 1] * myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Graves's Auto Attack");
            UpdateTotalDamage(ref aSum, 5, new Damage(BonusPelletADPercentByLevel[myStats.level - 1] * myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)71428), "Graves's Pellet");
            UpdateTotalDamage(ref aSum, 5, new Damage(BonusPelletADPercentByLevel[myStats.level - 1] * myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)71428), "Graves's Pellet");
            UpdateTotalDamage(ref aSum, 5, new Damage(BonusPelletADPercentByLevel[myStats.level - 1] * myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)71428), "Graves's Pellet");
        }
        myStats.eCD -= 2;
        if(trueGrit > 0)
        {
            StopCoroutine(TrueGrit());
            myStats.armor -= trueGrit * ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats);
            StartCoroutine(TrueGrit());
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public IEnumerator TrueGrit()
    {
        myStats.armor += trueGrit * ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats);
        yield return new WaitForSeconds(4);
        myStats.armor -= trueGrit * ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats);
        trueGrit = 0;
    }
}