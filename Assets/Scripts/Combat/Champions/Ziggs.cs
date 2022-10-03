using Simulator.Combat;
using System.Collections;

public class Ziggs : ChampionCombat
{
    public static float ShortFuseDamageByLevel(int level)
    {
        return level switch
        {
            < 7 => 16 + 4 * level,
            < 12 => 16 + 8 * level,
            _ => 16 + 12 * level,
        };

    }

    public static float ShortFuseCooldownReduceByLevel(int level)
    {
        return level switch
        {
            < 7 => 4,
            < 13 => 5,
            _ => 6,
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
        checksW.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        autoattackcheck = new ZiggsAACheck(this);

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage per Mine");
        rKeys.Add("Increased Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), 4, qKeys[0]);
        myStats.qCD = QSkill().basic.coolDown[4];
        myStats.pCD = myStats.passiveSkill.coolDown - ShortFuseCooldownReduceByLevel(myStats.level);
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        TargetBuffManager.Add("KnockOffBuff", new AirborneBuff(0.1f, TargetBuffManager, "SatchelCharge"));
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), 4, wKeys[0]);
        myStats.wCD = WSkill().basic.coolDown[4];
        myStats.pCD = myStats.passiveSkill.coolDown - ShortFuseCooldownReduceByLevel(myStats.level);
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), 4, eKeys[0]);
        myStats.eCD = ESkill().basic.coolDown[4];
        myStats.pCD = myStats.passiveSkill.coolDown - ShortFuseCooldownReduceByLevel(myStats.level);
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), 2, rKeys[0]);
        myStats.rCD = RSkill().basic.coolDown[2];
        myStats.pCD = myStats.passiveSkill.coolDown - ShortFuseCooldownReduceByLevel(myStats.level);
    }
}