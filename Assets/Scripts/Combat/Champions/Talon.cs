using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Talon : ChampionCombat
{
    private int passiveStack = 0;
    private CheckTalonP talonP;
    public bool isBleeding = false;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

        talonP = new CheckTalonP(this);
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

        qKeys.Add("Physical Damage");
        wKeys.Add("Total Physical Damage");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        passiveStack++;
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]); //need to apply crits
        attackCooldown = 0;
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        passiveStack += 2;
        UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        myStats.wCD = WSkill().basic.coolDown[4];
    }


    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        passiveStack++;
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        MyBuffManager.Add(RSkill().basic.name, new UntargetableBuff(2.5f, MyBuffManager, RSkill().basic.name));
        yield return new WaitForSeconds(2.5f);
        passiveStack++;
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        CheckTalonPassiveDamage();
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
    }

    private void CheckTalonPassiveDamage()
    {
        if (talonP.Control())
        {
            if (passiveStack == 3)
            {
                UpdateTotalDamage(ref pSum, 4, new Damage((65 + 10 * myStats.level), SkillDamageType.Phyiscal), myStats.passiveSkill.name);
                myStats.buffManager.buffs.Remove("BladeEnd");
            }
        }
        else
        {
            myStats.buffManager.buffs.Add("BladeEnd", new BleedBuff(2, TargetBuffManager, "BleedBuff"));
        }
    }
}