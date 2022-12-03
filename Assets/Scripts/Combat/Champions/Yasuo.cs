using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Yasuo : ChampionCombat
{
    public static float[] PassiveShieldByLevel = { 100f, 105f, 110, 115, 120, 130, 140, 150, 160, 170, 180, 200, 220, 250, 290, 350, 410, 475 };
    public static float pMaxCooldown = 12f;
    public float pCD = 0;
    public int qStack = 0;
    public bool OnTheE = false;
    public bool ActiveWindwall = false;
    public static float GetYasuoQCastTime(float bonusAS)
    {
        return bonusAS switch
        {
            < 15 => 0.4f,
            < 30 => 0.364f,
            < 45 => 0.328f,
            < 60 => 0.292f,
            < 75 => 0.256f,
            < 90 => 0.22f,
            < 105 => 0.184f,
            < 111.11f => 0.148f,
            _ => 0.133f
        };
    }
    public static float GetYasuoQCooldown(float bonusAS)
    {
        return bonusAS switch
        {
            < 15 => 4f,
            < 30 => 3.64f,
            < 45 => 3.28f,
            < 60 => 2.92f,
            < 75 => 2.56f,
            < 90 => 2.2f,
            < 105 => 1.84f,
            < 111.11f => 1.48f,
            _ => 1.33f
        };
    }

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
        checkTakeDamage.Add(new CheckIfBlockingProjectile(this, ref ActiveWindwall));
        checkTakeDamage.Add(new CheckYasuoPassive(this, this));
        checksR.Add(new CheckIfEnemyAirborne(this));
        checksQ.Add(new CheckIfUnableToAct(this));
        checksW.Add(new CheckIfUnableToAct(this));
        checksE.Add(new CheckIfUnableToAct(this));
        checksR.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Physical Damage");
        wKeys.Add("");
        eKeys.Add("Magic Damage");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        pCD -= Time.fixedDeltaTime;
    }
    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Yasuo's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(GetYasuoQCastTime(myStats.bonusAS)));
        if (qStack != 2) StartCoroutine(QStack());
        else
        {
            if (OnTheE) TargetBuffManager.Add("Airborne", new AirborneBuff(0.9f, targetStats.buffManager, myStats.qSkill[0].basic.name));
            else TargetBuffManager.Add("Airborne", new AirborneBuff(0.75f, targetStats.buffManager, myStats.qSkill[0].basic.name));
            qStack = 0;
            StopCoroutine(QStack());
			StopCoroutine(QStack());
		}
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)7048);
        myStats.qCD = GetYasuoQCooldown(myStats.bonusAS);
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;
        if (OnTheE) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal), WSkill().basic.name);
        StartCoroutine(ActivateWindwall());
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;
        if (OnTheE) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        StartCoroutine(RidingE());
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], skillComponentTypes:(SkillComponentTypes)34944);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (TargetBuffManager.buffs.ContainsKey("Airborne"))
        {
            if (myStats.rLevel == -1) yield break;
            if (!CheckForAbilityControl(checksR)) yield break;

            yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
            MyBuffManager.Add("UnableToAct", new UnableToActBuff(1f, myStats.buffManager, myStats.rSkill[0].basic.name));
            pCD = 0;
            qStack = 0;
            StopCoroutine(QStack());
            StopCoroutine(QStack());
            TargetBuffManager.buffs["Airborne"].duration = 1;
            UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)34816);
            StartCoroutine(UltimateArmorpen());
            myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
            simulationManager.AddCastLog(myCastLog, 4);
        }
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        if (!myStats.buffManager.buffs.ContainsKey("Airborne")) yield break;
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("UnableToAct", new UnableToActBuff(1f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        targetStats.buffManager.buffs["Airborne"].duration = 1;
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        //critics gain armor pen %50 percent
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
    }

    public IEnumerator QStack()
    {
        qStack++;
        yield return new WaitForSeconds(6f);
        qStack = 0;
    }

    public IEnumerator RidingE()
    {
        OnTheE = true;
        yield return new WaitForSeconds(0.5f);            //it is based on the movement
        OnTheE = false;
    }

    public IEnumerator UltimateArmorpen()
    {
        myStats.armorPenetrationPercent += 50;
        yield return new WaitForSeconds(15f);
        myStats.armorPenetrationPercent -= 50;
    }

    public IEnumerator ActivateWindwall()
    {
        ActiveWindwall = true;
        yield return new WaitForSeconds(4);
        ActiveWindwall = false;
    }
}