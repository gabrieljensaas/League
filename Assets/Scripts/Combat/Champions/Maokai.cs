using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Maokai : ChampionCombat
{
    private static float SapMagicCooldownByLevel(int level)
	{
        return level switch
        {
            < 6 => 30,
            < 11 => 25,
            _ => 20,
        };
	}
    private static float SapMagicHealingByLevel(int level)
	{
        return level switch
        {
            < 6 => 4,
            < 9 => 9,
            < 11 => 14,
            < 13 => 19,
            < 15 => 24,
            < 17 => 29,
            _ => 34,
        };
	}
    public float sapMagicCD = 0;
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
        checksW.Add(new CheckIfImmobilize(this));

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checkTakeDamage.Add(new CheckMaokaiPassive(this));
        checkTakeDamage.Add(new CheckMaokaiPassive(this));

        qKeys.Add("Magic Damage");

        wKeys.Add("Magic Damage");
        wKeys.Add("Root Duration");
        
        eKeys.Add("Magic Damage");
/*        eKeys.Add("Enhanced Damage Per Tick");*/ // Supposing not placed in bush

        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        sapMagicCD -= Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        TargetBuffManager.Add("Stun", new StunBuff(0.5f, TargetBuffManager, QSkill().basic.name));
        TargetBuffManager.Add("Airborne", new AirborneBuff(0.1f, TargetBuffManager, QSkill().basic.name));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes:SkillComponentTypes.Spellblockable, buffNames: new string[] { "Stun", "Airborne"});
        sapMagicCD -= 4;
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW) || myStats.wLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        TargetBuffManager.Add("Root", new RootBuff(WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), TargetBuffManager, WSkill().basic.name));
        UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable, buffNames: new string[] { "Root"});
        sapMagicCD -= 4;
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        //need to check if it is placed in a bush or not. Could be mistake on Ivern case as it creates bush
        yield return new WaitForSeconds(2.5f);
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes:SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile | SkillComponentTypes.Blockable);
        sapMagicCD -= 4;
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        //Distance Travelled = 0
        TargetBuffManager.Add("Root", new RootBuff(0.8f, TargetBuffManager, RSkill().basic.name));
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable | SkillComponentTypes.Blockable, buffNames: new string[] {"Root"});
        sapMagicCD -= 4;
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if(sapMagicCD <= 0 && myStats.PercentCurrentHealth < 0.95f)
		{
            UpdateTotalHeal(ref pSum, SapMagicHealingByLevel(myStats.level), myStats.passiveSkill.skillName);
            sapMagicCD = SapMagicCooldownByLevel(myStats.level);
            attackCooldown = 0;
        }
    }
}
public class CheckMaokaiPassive : Check
{
    private Maokai maokai;
    public CheckMaokaiPassive(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        maokai.sapMagicCD -= 4;
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}