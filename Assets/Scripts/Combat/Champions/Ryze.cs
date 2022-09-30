using Simulator.Combat;
using System.Collections;

public class Ryze : ChampionCombat
{
    private static float RyzeRPassiveDamageByLevel(int level)
	{
        if (level == 0) return 0.1f;
        if (level == 1) return 0.4f;
        if (level == 2) return 0.7f;
        else return 1f;
    }

    public bool hasFlux = false;
    
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "Q", "W", "R", "A" };

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

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if(!hasFlux)
		{
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
        }
		else
		{
            float increasedDamage = QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats) * RyzeRPassiveDamageByLevel(myStats.rLevel);
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats) + increasedDamage, QSkill().basic.name, SkillDamageType.Spell);

        }

        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        if (hasFlux)
        {
            MyBuffManager.Add("RootBuff", new RootBuff(1.5f, TargetBuffManager, "RootBuff"));
        }
        myStats.qCD = 0;
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
        MyBuffManager.Add("FluxBuff", new FluxBuff(3, TargetBuffManager));
        myStats.qCD = 0;
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        myStats.qCD = 0;
        myStats.rCD = RSkill().basic.coolDown[2];
    }

}