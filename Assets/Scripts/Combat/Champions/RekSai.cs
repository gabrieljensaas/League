using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class RekSai : ChampionCombat
{
    private int furyPassive = 0;
    public int autoattackQ;
    public float timeSinceQ;
    public bool isBorrowed;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "W", "E", "R", "A" };

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
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        autoattackcheck = new RekSaiAACheck(this, this);

        qKeys.Add("Bonus Physical Damage");
        qKeys.Add("Physical Damage");
        wKeys.Add("Physical Damage");
        eKeys.Add("Physical Damage");
        eKeys.Add("True Damage");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceQ += Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if (!isBorrowed)
        {
            autoattackQ = 3;
            timeSinceQ = 0;
            if (timeSinceQ > 5)
            {
                autoattackQ = 0;
            }
            myStats.qCD = 4;

        }
        else
        {
            UpdateTotalDamage(ref qSum, 1, QSkill(), myStats.qLevel, qKeys[1]);
            myStats.qCD = QSkill().basic.coolDown[4];
            furyPassive += 25;
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        if (!isBorrowed)
        {
            isBorrowed = true;
            myStats.wCD = 4;
        }
        else
        {
            isBorrowed = false;
            UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
            TargetBuffManager.Add("KnockOff", new AirborneBuff(0.1f, TargetBuffManager, "KnockOff"));
            myStats.wCD = 1;
            furyPassive += 25;
        }

    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        if (!isBorrowed)
        {
            if (furyPassive == 100)
            {
                UpdateTotalDamage(ref eSum, 2, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), SkillDamageType.True), ESkill().basic.name);
            }
            else
            {
                UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
            }
            furyPassive += 25;
            myStats.eCD = ESkill().basic.coolDown[4];
        }

    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        MyBuffManager.Add("Untargetable", new UntargetableBuff(1.5f, MyBuffManager, "Untargetable"));
        yield return new WaitForSeconds(1.5f);
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        furyPassive += 25;
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        furyPassive += 25;
    }
}