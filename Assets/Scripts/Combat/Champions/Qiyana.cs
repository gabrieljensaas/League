using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Qiyana : ChampionCombat
{
    private bool hasPassive;
    private bool hasTerrain;
    private bool hasRiver;
    private bool hasBrush;
    private float passiveCooldown;
    private bool hasElement;
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
        checksE.Add(new CheckIfImmobilize(this));

        qKeys.Add("Physical Damage");
        qKeys.Add("Increased Damage");
        wKeys.Add("Bonus Attack Speed");
        wKeys.Add("Bonus Magic Damage");
        eKeys.Add("Physical Damage");
        rKeys.Add("Physical Damage");

        if (hasElement)
        {
            MyBuffManager.Add("AttackSpeedBuff", new AttackSpeedBuff(float.MaxValue, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), "AttackSpeedBuff"));
        }

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        passiveCooldown -= Time.fixedDeltaTime;
    }
    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        RoyalPrivilege();
        if (hasElement)
        {
            UpdateAbilityTotalDamage(ref wSum, 1, new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.OnHit), WSkill().basic.name);
        }
        if (hasPassive)
        {
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage(11 + 4 * myStats.level, SkillDamageType.Phyiscal), myStats.passiveSkill.name);
            hasPassive = false;
            if (hasTerrain)
            {
                UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[1], myStats, targetStats), SkillDamageType.Phyiscal), QSkill().basic.name);
                hasTerrain = false;
                hasElement = false;
            }
            else if (hasBrush)
            {
                MyBuffManager.Add("Invisible", new UntargetableBuff(3, MyBuffManager, "Invisible"));
                UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[0], myStats, targetStats), SkillDamageType.Phyiscal), QSkill().basic.name);
                hasBrush = false;
                hasElement = false;
            }
            else if (hasRiver)
            {
                MyBuffManager.Add("RootBuff", new RootBuff(3, MyBuffManager, "RiverBuff"));
                UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[0], myStats, targetStats), SkillDamageType.Phyiscal), QSkill().basic.name);
                hasRiver = false;
                hasElement = false;
            }
            else
                UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[0], myStats, targetStats), SkillDamageType.Phyiscal), QSkill().basic.name);

        }
        else
        {
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[0], myStats, targetStats), SkillDamageType.Phyiscal), QSkill().basic.name);
        }
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        if (targetStats.PercentMissingHealth < 0.5)
        {
            hasTerrain = true;
        }
        else
        {
            int check = UnityEngine.Random.Range(0, 1);
            if (check == 1)
            {
                hasBrush = true;
            }
            else if (check == 0)
            {
                hasRiver = true;
            }
        }
        hasElement = true;
        passiveCooldown = 0;
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        if (hasElement)
        {
            UpdateAbilityTotalDamage(ref wSum, 1, new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.OnHit), WSkill().basic.name);
        }
        UpdateAbilityTotalDamage(ref eSum, 0, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), SkillDamageType.PhysAndSpell, SkillComponentTypes.Dash), ESkill().basic.name);
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        if (hasElement)
        {
            UpdateAbilityTotalDamage(ref wSum, 1, new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.OnHit), WSkill().basic.name);
        }
        TargetBuffManager.Add("Airborne", new AirborneBuff(0.1f, TargetBuffManager, RSkill().basic.name));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        float check = UnityEngine.Random.Range(0.5f, 1); ;
        TargetBuffManager.Add("Stun", new StunBuff(check, TargetBuffManager, RSkill().basic.name));
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (hasElement)
        {
            AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.OnHit));
        }
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
    }
    public void RoyalPrivilege()
    {
        if (passiveCooldown <= 0)
        {
            hasPassive = true;
            passiveCooldown = 25;
        }

    }
}