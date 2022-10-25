using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Jayce : ChampionCombat
{
    private bool hammerMode = true;
    private bool gotHammerBuff = false;
    private bool gotCannonBuff = false;
    private bool acceleratorGate = false;
    private bool cannonQReady = true;
    private bool cannonWReady = true;
    private bool cannonEReady = true;
    private bool cannonRReady = true;
    private int hyperCharge = 0;

    public static float GetJayceRHammerPenetrationByLevel(int level)
    {
        return level switch
        {
            < 6 => 10,
            < 11 => 15,
            < 16 => 20,
            _ => 25
        };
    }

    public static float GetJayceRCannonResistanceByLevel(int level)
    {
        return level switch
        {
            < 6 => 5,
            < 11 => 15,
            < 16 => 25,
            _ => 35
        };
    }

    public static float GetJayceRCannonDamageByLevel(int level)
    {
        return level switch
        {
            < 6 => 25,
            < 11 => 65,
            < 16 => 105,
            _ => 145
        };
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "W", "E", "Q", "R", "A" };

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

        qKeys.Add("Physical Damage");
        qKeys.Add("Physical Damage");
        qKeys.Add("Increased Damage");
        wKeys.Add("Magic Damage Per Tick");
        wKeys.Add("Damage Modifier");
        eKeys.Add("Magic Damage");

        myStats.buffManager.buffs.Add(myStats.rSkill[1].basic.name, new ArmorBuff(float.MaxValue, myStats.buffManager, myStats.rSkill[1].basic.name, (int)GetJayceRCannonResistanceByLevel(myStats.level), myStats.rSkill[1].basic.name));
        myStats.buffManager.buffs.Add(myStats.rSkill[1].basic.name + " ", new MagicResistanceBuff(float.MaxValue, myStats.buffManager, myStats.rSkill[1].basic.name, (int)GetJayceRCannonResistanceByLevel(myStats.level), myStats.rSkill[1].basic.name));

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (hammerMode)
        {
            if (!CheckForAbilityControl(checksQ)) yield break;
            if (myStats.buffManager.HasImmobilize) yield break;
            if (myStats.qCD > 0) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
            UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        }
        else
        {
            if (!CheckForAbilityControl(checksQ)) yield break;
            if (!cannonQReady) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.qSkill[1].basic.castTime));
            if (acceleratorGate) UpdateTotalDamage(ref qSum, 0, myStats.qSkill[1], 4, qKeys[2]);
            else UpdateTotalDamage(ref qSum, 0, myStats.qSkill[1], 4, qKeys[1]);
            StartCoroutine(CannonQCD());
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (hammerMode)
        {
            if (!CheckForAbilityControl(checksW)) yield break;
            if (myStats.wCD > 0) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
            StartCoroutine(LightiningField());
            myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        }
        else
        {
            if (!CheckForAbilityControl(checksW)) yield break;
            if (!cannonWReady) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.wSkill[1].basic.castTime));
            StartCoroutine(HyperCharge());
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (hammerMode)
        {
            if (!CheckForAbilityControl(checksE)) yield break;
            if (myStats.eCD > 0) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.1f, targetStats.buffManager, myStats.eSkill[0].basic.name));
            myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        }
        else
        {
            if (!CheckForAbilityControl(checksE)) yield break;
            if (!cannonEReady) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[1].basic.castTime));
            StartCoroutine(AccelerationGate());
            StartCoroutine(CannonECD());
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (hammerMode)
        {
            if (!CheckForAbilityControl(checksR)) yield break;
            if (myStats.rCD > 0) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
            hammerMode = false;
            gotCannonBuff = false;
            gotHammerBuff = true;
            myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
            myStats.buffManager.buffs.Remove(myStats.rSkill[1].basic.name);
            myStats.buffManager.buffs.Remove(myStats.rSkill[1].basic.name + " ");
        }
        else
        {
            if (!CheckForAbilityControl(checksR)) yield break;
            if (!cannonRReady) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[1].basic.castTime));
            myStats.buffManager.buffs.Add(myStats.rSkill[1].basic.name, new ArmorBuff(float.MaxValue, myStats.buffManager, myStats.rSkill[1].basic.name, (int)GetJayceRCannonResistanceByLevel(myStats.level), myStats.rSkill[1].basic.name));
            myStats.buffManager.buffs.Add(myStats.rSkill[1].basic.name + " ", new MagicResistanceBuff(float.MaxValue, myStats.buffManager, myStats.rSkill[1].basic.name, (int)GetJayceRCannonResistanceByLevel(myStats.level), myStats.rSkill[1].basic.name));
            hammerMode = true;
            gotHammerBuff = false;
            gotCannonBuff = true;
            StartCoroutine(CannonRCD());
        }
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (gotHammerBuff)
        {
            targetStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new ArmorReductionBuff(5, targetStats.buffManager, myStats.rSkill[0].basic.name, GetJayceRHammerPenetrationByLevel(myStats.level), myStats.rSkill[0].basic.name));
            targetStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new MagicResistanceReductionBuff(5, targetStats.buffManager, myStats.rSkill[0].basic.name, GetJayceRHammerPenetrationByLevel(myStats.level), myStats.rSkill[0].basic.name));
            gotHammerBuff = false;
        }
        AutoAttack(new Damage(hyperCharge > 0 ? myStats.wSkill[1].UseSkill(4, wKeys[1], myStats, targetStats) : myStats.AD, SkillDamageType.Phyiscal));
        if (gotCannonBuff)
        {
            UpdateTotalDamage(ref rSum, 3, new Damage(GetJayceRCannonDamageByLevel(myStats.level) + (myStats.bonusAD * 0.25f), SkillDamageType.Spell), myStats.rSkill[1].basic.name);
            gotCannonBuff = false;
        }

        if (hyperCharge == 1)
        {
            hyperCharge = 0;
            myStats.buffManager.buffs.Remove(myStats.wSkill[1].basic.name);
            StartCoroutine(CannonWCD());
        }
        else hyperCharge--;
    }

    public IEnumerator LightiningField()
    {
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        yield return new WaitForSeconds(1f);
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
    }

    public IEnumerator CannonQCD()
    {
        cannonQReady = false;
        yield return new WaitForSeconds(myStats.qSkill[1].basic.coolDown[4]);
        cannonQReady = true;
    }

    public IEnumerator CannonWCD()
    {
        cannonWReady = false;
        yield return new WaitForSeconds(myStats.wSkill[1].basic.coolDown[4]);
        cannonWReady = true;
    }

    public IEnumerator CannonECD()
    {
        cannonEReady = false;
        yield return new WaitForSeconds(myStats.eSkill[1].basic.coolDown[4]);
        cannonEReady = true;
    }

    public IEnumerator CannonRCD()
    {
        cannonRReady = false;
        yield return new WaitForSeconds(myStats.rSkill[1].basic.coolDown[2]);
        cannonRReady = true;
    }

    public IEnumerator HyperCharge()
    {
        hyperCharge = 3;
        cannonWReady = false;
        myStats.buffManager.buffs.Add(myStats.wSkill[1].basic.name, new AttackSpeedBuff(float.MaxValue, myStats.buffManager, myStats.wSkill[1].basic.name, 3, myStats.wSkill[1].basic.name));
        yield return new WaitForSeconds(4);
        hyperCharge = 0;
        myStats.buffManager.buffs.Remove(myStats.wSkill[1].basic.name);
        StartCoroutine(CannonWCD());
    }

    public IEnumerator AccelerationGate()
    {
        acceleratorGate = true;
        yield return new WaitForSeconds(4);
        acceleratorGate = false;
    }
}