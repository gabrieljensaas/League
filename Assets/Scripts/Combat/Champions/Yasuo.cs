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
        checkTakeDamageAA.Add(new CheckYasuoPassive(this, this));
        checkTakeDamageAbility.Add(new CheckYasuoPassive(this, this));
        checksR.Add(new CheckIfEnemyAirborne(this));
        checksQ.Add(new CheckIfUnableToAct(this));
        checksW.Add(new CheckIfUnableToAct(this));
        checksE.Add(new CheckIfUnableToAct(this));
        checksR.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));

        myStats.qSkill[0].basic.castTime = GetYasuoQCastTime(myStats.bonusAS);
        myStats.qSkill[0].basic.coolDown[0] = GetYasuoQCooldown(myStats.bonusAS);
        myStats.qSkill[0].basic.coolDown[1] = GetYasuoQCooldown(myStats.bonusAS);
        myStats.qSkill[0].basic.coolDown[2] = GetYasuoQCooldown(myStats.bonusAS);
        myStats.qSkill[0].basic.coolDown[3] = GetYasuoQCooldown(myStats.bonusAS);
        myStats.qSkill[0].basic.coolDown[4] = GetYasuoQCooldown(myStats.bonusAS);

        qKeys.Add("Physical Damage");
        wKeys.Add("");
        eKeys.Add("Magic Damage");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        pCD -= Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if (qStack != 2) StartCoroutine(QStack());
        else
        {
            if (OnTheE) targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.9f, targetStats.buffManager, myStats.qSkill[0].basic.name));
            else targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.75f, targetStats.buffManager, myStats.qSkill[0].basic.name));
            qStack = 0;
            StopCoroutine(QStack());
            StopCoroutine(QStack());
        }
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;
        if (OnTheE) yield break;
        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        //blocks projectiles add later
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;
        if (OnTheE) yield break;
        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        StartCoroutine(RidingE());
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("UnableToAct", new UnableToActBuff(1f, myStats.buffManager, myStats.rSkill[0].basic.name));
        pCD = 0;
        qStack = 0;
        StopCoroutine(QStack());
        StopCoroutine(QStack());
        targetStats.buffManager.buffs["Airborne"].duration = 1;
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        //critics gain armor pen %50 percent
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
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
}