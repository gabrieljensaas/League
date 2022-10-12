using Simulator.Combat;
using System.Collections;

public class XinZhao : ChampionCombat
{
    private static float DeterminationDamageByLevel(int level)
	{
        return level switch
        {
            < 6 => 0.15f,
            < 11 => 0.25f,
            < 16 => 0.35f,
            _ => 0.45f,
        };
	}

    private int pStacks;
    private int talonStrikeAuto;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "W", "E", "R", "Q", "A" };

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


        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Bonus Physical Damage");

        wKeys.Add("Slash Physical Damage");
        wKeys.Add("Thrust Physical Damage");

        eKeys.Add("Magic Damage");
        eKeys.Add("Bonus Attack Speed");

        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        talonStrikeAuto = 3;
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[1]); // need to apply crits
        pStacks++;
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
        MyBuffManager.Add("AttackSpeedBuff", new AttackSpeedBuff(5f, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), "AudaciousCharge"));
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        pStacks++;
        if (pStacks == 3)
		{
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage(DeterminationDamageByLevel(myStats.level), SkillDamageType.Phyiscal, SkillComponentTypes.ProcDamage), myStats.passiveSkill.skillName);
            UpdateTotalHeal(ref pSum, 2 + 4 * myStats.level, myStats.passiveSkill.skillName);
		}
        if(talonStrikeAuto > 0)
		{            
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.Phyiscal, SkillComponentTypes.ProcDamage), myStats.passiveSkill.skillName);
            myStats.wCD--;
            myStats.eCD--;
            myStats.rCD--;
            if (talonStrikeAuto == 1) TargetBuffManager.Add("ThreeTalonStrike", new AirborneBuff(0.75f, TargetBuffManager, QSkill().basic.name));
            talonStrikeAuto--;  
        }
    }
}