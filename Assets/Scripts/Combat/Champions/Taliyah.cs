using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Taliyah : ChampionCombat
{
    private bool hasQActive = true;
    private float timeSinceW;

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
        checksR.Add(new CheckIfImmobilize(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Reduced Damage");
        qKeys.Add("Empowered Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("Total Maximum Detonation Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceW += Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if (hasQActive)
        {
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[0], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Projectile), QSkill().basic.name);
            yield return new WaitForSeconds(0.3f);
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[1], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Projectile), QSkill().basic.name);
            yield return new WaitForSeconds(0.3f);
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[1], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Projectile), QSkill().basic.name);
            yield return new WaitForSeconds(0.3f);
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[1], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Projectile), QSkill().basic.name);
            yield return new WaitForSeconds(0.3f);
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[1], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Projectile), QSkill().basic.name);
            yield return new WaitForSeconds(0.3f);
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[1], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Projectile), QSkill().basic.name);
            myStats.qCD = QSkill().basic.coolDown[4];
            hasQActive = !hasQActive;
        }
        else
        {
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[2], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Projectile), QSkill().basic.name);
            myStats.qCD = QSkill().basic.coolDown[4] / 2;
            if (myStats.qCD < 0.75) myStats.qCD = 0.75f;
            hasQActive = !hasQActive;
        }


    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        TargetBuffManager.Add("SeismicShove", new AirborneBuff(1f, TargetBuffManager, WSkill().basic.name));
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
        timeSinceW = 0;
        if (timeSinceW <= 4 && TargetBuffManager.buffs.TryGetValue("Airborne", out Buff airborne)) //need to check if enemy has a dash or not
        {
            TargetBuffManager.Add("StunBuff", new StunBuff(2f, TargetBuffManager, ESkill().basic.name));
            UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[1]);
        }
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
    }
}