using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Nautilus : ChampionCombat
{
    private static float StaggeringBlowRootDuration(int level)
	{
        return level switch
        {
            <= 1 => 0.75f,
            <= 6 => 1f,
            <= 11 => 1.25f,
            _ => 1.5f
        };
	}
    private float timeSinceStaggeringBlow = 6;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "W", "E", "A" };

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

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");

        wKeys.Add("Shield Strength");
        wKeys.Add("Magic Damage");
        
        eKeys.Add("Total Single-Target Damage");
        
        rKeys.Add("Magic Damage");
        rKeys.Add("Stun Duration");

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceStaggeringBlow += Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.Spell), QSkill().basic.name);
        TargetBuffManager.Add("AirBorneBuff", new AirborneBuff(0.2f, TargetBuffManager, QSkill().basic.name));
        TargetBuffManager.Add("RootBuff", new RootBuff(0.1f, TargetBuffManager,QSkill().basic.name));
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW) || myStats.wLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        attackCooldown = 0;
        MyBuffManager.Add("ShieldBuff", new ShieldBuff(6, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), "TitanWarth"));
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        yield return new WaitForSeconds(1.5f);
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        TargetBuffManager.Add("AirborneBuff", new AirborneBuff(1f, TargetBuffManager, RSkill().basic.name));
        TargetBuffManager.Add("StunBuff", new StunBuff(RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats), TargetBuffManager, RSkill().basic.name));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if(timeSinceStaggeringBlow > 6 && AutoAttack(new Damage(2 + 6 * myStats.level, SkillDamageType.Phyiscal, SkillComponentTypes.ProcDamage)).damage != float.MinValue)
		{
            TargetBuffManager.Add("RootBuff", new RootBuff(StaggeringBlowRootDuration(myStats.level), TargetBuffManager, myStats.passiveSkill.skillName));
            timeSinceStaggeringBlow = 0;
        }
        if (MyBuffManager.buffs.ContainsKey("TitanWarth"))
        {
            AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats) * 0.5f, SkillDamageType.Spell, SkillComponentTypes.ProcDamage));
            yield return new WaitForSeconds(1.25f);
            AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats) * 0.5f, SkillDamageType.Spell, SkillComponentTypes.ProcDamage));
        }
    }
}