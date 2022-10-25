using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Blitzcrank : ChampionCombat
{
    private bool hasPowerFist;
	private bool hasStaticFieldPassive;

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

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Magic Damage");

        wKeys.Add("Bonus Attack Speed");

        rKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        //Need to add manabarrier when Hp less than 30%S

        if (myStats.rCD <= 0)
            hasStaticFieldPassive = true;
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        TargetBuffManager.Add("Stun", new StunBuff(0.65f, TargetBuffManager, QSkill().basic.name));
        TargetBuffManager.Add("Airborne", new AirborneBuff(0.1f, TargetBuffManager, QSkill().basic.name));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable | SkillComponentTypes.Blockable, buffNames: new string[] { "Stun", "Airborne"});
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW) || myStats.wLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));

        MyBuffManager.Add("AttackSpeed", new AttackSpeedBuff(5, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), WSkill().basic.name));
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        hasPowerFist = true;
        attackCooldown = 0;
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
        if (TargetBuffManager.buffs.TryGetValue("Shield", out Buff value))
        {
            value.Kill();
        }
        TargetBuffManager.Add("Silence", new SilenceBuff(0.5f, TargetBuffManager, RSkill().basic.name));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if (MyBuffManager.buffs.ContainsKey("AttackSpeed"))
        {
            AutoAttack(new Damage(0.01f * targetStats.maxHealth, SkillDamageType.Spell, SkillComponentTypes.OnHit));
        }
        if (hasPowerFist && AutoAttack(new Damage(myStats.AD * 0.75f, SkillDamageType.Phyiscal, SkillComponentTypes.ProcDamage | SkillComponentTypes.Spellblockable)).damage != float.MinValue)
        {
            TargetBuffManager.Add("Airborne", new AirborneBuff(1f, TargetBuffManager, ESkill().basic.name));
            hasPowerFist = false;
        }
        if(hasStaticFieldPassive)
		{
            yield return new WaitForSeconds(1f);
            UpdateTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), SkillDamageType.Spell), RSkill().basic.name);
		}

    }
}