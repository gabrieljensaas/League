using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Riven : ChampionCombat
{
    private int qCounter = 0;
    private float timeSinceLastQ = 0f;
    private float timeSinceR = 0f;
    private bool hasWindSlash = false;
    private CheckIfExecutes r1ExecuteCheck;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "E", "Q", "A" };
        checksQ.Add(new CheckIfCasting(this));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        autoattackcheck = new RivenAACheck(this);
        targetCombat.checksQ.Add(new CheckIfAirborne(targetCombat));
        targetCombat.checksW.Add(new CheckIfAirborne(targetCombat));
        targetCombat.checksE.Add(new CheckIfAirborne(targetCombat));
        targetCombat.checksR.Add(new CheckIfAirborne(targetCombat));
        targetCombat.checksA.Add(new CheckIfAirborne(targetCombat));
        targetCombat.checksQ.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksW.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksE.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksR.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksA.Add(new CheckIfStunned(targetCombat));
        r1ExecuteCheck = new CheckIfExecutes(this, "R1");

        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        timeSinceLastQ += Time.deltaTime;
        timeSinceR += Time.deltaTime;
        if (timeSinceR > 15) hasWindSlash = false;
    }

    public override IEnumerator ExecuteQ()
    {
        if (qCounter == 0 && myStats.qCD > 0) yield break;
        if (timeSinceLastQ < 0.3125f) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;
        if (myStats.buffManager.HasImmobilize) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys);
        UpdateRivenPassive();
        if (qCounter == 0)
        {
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
            timeSinceLastQ = 0f;
            qCounter++;
        }
        else if (qCounter == 2)
        {
            timeSinceLastQ = 0f;
            qCounter = 0;
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.5f, targetStats.buffManager, myStats.qSkill[0].basic.name));
        }
        else
        {
            timeSinceLastQ = 0f;
            qCounter++;
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys);
        UpdateRivenPassive();
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;
        if (myStats.buffManager.HasImmobilize) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys);
        UpdateRivenPassive();
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        if (hasWindSlash && (r1ExecuteCheck.Control() || timeSinceR > 14))
        {
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[1].basic.castTime));
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[1], 2, rKeys);
            UpdateRivenPassive();
            myStats.rCD = myStats.rSkill[0].basic.coolDown[2] - timeSinceR;
            hasWindSlash = false;
        }

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        yield return StartCoroutine(StartCastingAbility(0.5f - myStats.rSkill[0].basic.castTime));
        UpdateRivenPassive();
        myStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new AttackDamageBuff(15, myStats.buffManager, myStats.rSkill[0].basic.name, (int)(myStats.AD * 0.2f), myStats.rSkill[0].basic.name));
        hasWindSlash = true;
        timeSinceR = 0f;
    }

    private void UpdateRivenPassive()
    {
        if (myStats.buffManager.buffs.TryGetValue("RunicBlade", out Buff value))
        {
            if (value.value == 3) value.duration = 6;
            else
            {
                value.value++;
                value.duration = 6;
            }
        }
        else myStats.buffManager.buffs.Add("RunicBlade", new RunicBladeBuff(6, myStats.buffManager, myStats.passiveSkill.skillName));
    }
}