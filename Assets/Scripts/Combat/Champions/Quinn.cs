using Simulator.Combat;
using System.Collections;

public class Quinn : ChampionCombat
{
    public static float HarrierCooldownByCrit(float crit)
    {
        return crit switch
        {
            < 10 => 8f,
            < 20 => 7.24f,
            < 30 => 6.54f,
            < 40 => 5.92f,
            < 50 => 5.35f,
            < 60 => 4.84f,
            < 70 => 4.38f,
            < 80 => 3.96f,
            < 90 => 3.58f,
            < 100 => 3.28f,
            _ => 2.93f,

        };
    }
    private bool hasSkyStrike = false;
    private bool rCast;
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

        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));

        qKeys.Add("Physical Damage");
        wKeys.Add("Bonus Attack Speed");
        eKeys.Add("Physical damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        TargetBuffManager.Add("HarrierVulnerable", new HarrierVulnerableBuff(4, TargetBuffManager, QSkill().basic.name));
        HarrierVulnerable(QSkill().basic.name);
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]); UpdateTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.True), QSkill().basic.name);
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        TargetBuffManager.Add("HarrierVulnerable", new HarrierVulnerableBuff(4, TargetBuffManager, WSkill().basic.name));
        HarrierVulnerable(WSkill().basic.name);
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
        TargetBuffManager.Add("KnockBackBuff", new AirborneBuff(0.1f, TargetBuffManager, ESkill().basic.name));
        TargetBuffManager.Add("HarrierVulnerable", new HarrierVulnerableBuff(4, TargetBuffManager, ESkill().basic.name));
        HarrierVulnerable(ESkill().basic.name);
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        if (!rCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
            MyBuffManager.Add("Channeling", new ChannelingBuff(2, MyBuffManager, RSkill().basic.name, "Channeling"));
            myStats.rCD = 2f;
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
            hasSkyStrike = true;
        }
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (targetStats.buffManager.buffs.TryGetValue("HarrierVulnerable", out Buff buff))
        {
            UpdateTotalDamage(ref pSum, 4, new Damage(5 + 5 * myStats.level, SkillDamageType.Phyiscal), myStats.passiveSkill.skillName);
            MyBuffManager.Add("AttackSpeedBuff", new AttackSpeedBuff(2f, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), "AttackSppedBuff"));
            buff.Kill();
            myStats.pCD = HarrierCooldownByCrit(myStats.critStrikeChance);
        }
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
    }

    void HarrierVulnerable(string source)
    {
        if (myStats.pCD == 0)
        {
            if (targetStats.buffManager.buffs.TryGetValue("HarrierVulnerable", out Buff buff))
                buff.duration = 4;
            else
                TargetBuffManager.Add("HarrierVulnerable", new HarrierVulnerableBuff(4, TargetBuffManager, source));
        }
    }
    public void CheckiFSkyStrike()
    {
        if (hasSkyStrike)
        {
            UpdateTotalDamage(ref rSum, 3, new Damage(0.7f * myStats.AD, SkillDamageType.Phyiscal), RSkill().basic.name);
            hasSkyStrike = false;
        }
    }
    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }
}