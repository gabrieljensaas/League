using Simulator.Combat;
using System.Collections;
using UnityEngine;
/// <summary>
/// needs more in depth research for reload speed it is not much detailed in wiki
/// </summary>
public class Graves : ChampionCombat
{
    public float timeSinceLastAA = 0f;
    public int currentAmmo = 2;
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

        qKeys.Add("Physical Damage");
        qKeys.Add("Physical Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        timeSinceLastAA += Time.fixedDeltaTime;

        if (currentAmmo != 2)
        {
            if (currentAmmo == 1 && timeSinceLastAA >= 1.3f)
            {
                currentAmmo = 2;
                timeSinceLastAA = 0;
            }
            else if (timeSinceLastAA >= 2.08)              //reload speed
            {
                currentAmmo = 2;
                timeSinceLastAA = 0;
            }
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;
        timeSinceLastAA = 0;
        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        timeSinceLastAA = 0;
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(2f);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[1]);
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;
        timeSinceLastAA = 0;
        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        timeSinceLastAA = 0;
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        timeSinceLastAA = 0;
        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        timeSinceLastAA = 0;
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;
        timeSinceLastAA = 0;
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        timeSinceLastAA = 0;
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
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
            AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        }
        else if (currentAmmo == 1)
        {
            AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        }
    }
}