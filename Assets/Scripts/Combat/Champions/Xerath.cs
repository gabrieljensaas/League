using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Xerath : ChampionCombat
{
    private bool qCast;
    private float timeSinceQ;

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

        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));

        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Magic Damage");
        wKeys.Add("Increased Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Number of Recasts");

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceQ += Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        if (!qCast)
        {
            yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
            myStats.qCD = 0.528f;
            timeSinceQ = 0;
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
            myStats.qCD = QSkill().basic.coolDown[4] - timeSinceQ;
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
        TargetBuffManager.Add("StunBuff", new StunBuff(0.75f, TargetBuffManager, "StunBuff"));
        myStats.eCD = ESkill().basic.coolDown[4];
    }


    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        MyBuffManager.Add("Channeling", new ChannelingBuff(10, MyBuffManager, RSkill().basic.name, "RiteOfArcane"));
        yield return StartCoroutine(RiteOfArcane());
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    private IEnumerator RiteOfArcane()
    {
        int reCast = (int) RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats);
        while (reCast > 0)
        {
                UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]); ;

            reCast--;
            yield return new WaitForSeconds(0.25f);
        }
        if(reCast == (int)RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats))
		{
            myStats.rCD = myStats.rCD / 2;
		}

        myStats.buffManager.buffs.Remove("Channeling");
    }
    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }
}