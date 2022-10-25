using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Nami : ChampionCombat
{
    private float timeSinceBlessing;
    private int blessing;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "R", "Q", "W", "A" };

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

        wKeys.Add("Heal");
        wKeys.Add("Minimum Heal");
        wKeys.Add("Magic Damage");
        wKeys.Add("Minimum Damage");
        
        eKeys.Add("Bonus Magic Damage Per Hit");
        
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceBlessing += Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        yield return new WaitForSeconds(0.726f);
        if (blessing > 0 && timeSinceBlessing < 6)
		{
            UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: SkillComponentTypes.ProcDamage | SkillComponentTypes.Blockable);
            blessing--;
        }
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable, buffNames: new string[] {"Suspension"});
        TargetBuffManager.Add("Suspension", new SuspensionBuff(1.5f, TargetBuffManager, QSkill().basic.name));
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW) || myStats.wLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        if (myStats.PercentCurrentHealth < 0.3)
		{
            UpdateTotalHeal(ref wSum, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), WSkill().basic.name);
            UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[3], skillComponentTypes:SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable);
        }
        else
		{
            UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[2], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable);
            UpdateTotalHeal(ref wSum, WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), WSkill().basic.name);
        }
        if (blessing > 0 && timeSinceBlessing < 6)
        {
            UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: SkillComponentTypes.ProcDamage | SkillComponentTypes.Blockable);
            blessing--;
        }

        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        timeSinceBlessing = 0;
        blessing = 3;
        yield return new WaitForSeconds(6f);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable, buffNames: new string[] { "Airborne"});
        TargetBuffManager.Add("Airborne", new AirborneBuff(0.5f, TargetBuffManager, RSkill().basic.name));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if (blessing > 0 && timeSinceBlessing <6 && AutoAttack(new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.ProcDamage | SkillComponentTypes.Blockable)).damage != float.MinValue)
		{
            blessing--;
		}
    }
}