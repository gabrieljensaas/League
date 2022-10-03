using Simulator.Combat;
using System.Collections;

public class Lissandra : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "E", "W", "R", "A" };

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

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage");
        wKeys.Add("Root Duration");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Minimum Total Heal");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), 4, qKeys[0]);
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), 4, wKeys[0]);
        MyBuffManager.Add("RootBuff", new RootBuff(WSkill().UseSkill(4, wKeys[1], myStats, targetStats), TargetBuffManager, "RootBuff"));
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), 4, eKeys[0]);
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        if (myStats.PercentCurrentHealth < 0.2)
        {
            MyBuffManager.Add("StasisBuff", new StasisBuff(2.5f, MyBuffManager, RSkill().basic.name));
            UpdateTotalHeal(ref rSum, RSkill().UseSkill(2, rKeys[1], myStats, targetStats) + (RSkill().UseSkill(2, rKeys[1], myStats, targetStats) * myStats.PercentMissingHealth), RSkill().basic.name);
            UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), 2, rKeys[0]);
        }
        else
        {
            UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), 2, rKeys[0]);
            MyBuffManager.Add("KnockDownBuff", new KnockdownBuff(0.1f, TargetBuffManager, RSkill().basic.name));
            MyBuffManager.Add("StunBuff", new StunBuff(1.5f, TargetBuffManager, RSkill().basic.name));
        }
        myStats.rCD = RSkill().basic.coolDown[2];
    }

}