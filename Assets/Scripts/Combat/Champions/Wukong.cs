using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Wukong : ChampionCombat
{
    public int pStack;
    private bool hasCrushingBlow;
    private float timeSinceCrushingBlowActive;
    private bool hasClone;

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

        qKeys.Add("Bonus Physical Damage");
        qKeys.Add("Armor Reduction");

        wKeys.Add("Clone Outgoing Damage");

        eKeys.Add("Magic Damage");
        eKeys.Add("Bonus Attack Speed");

        rKeys.Add("Physical Damage Per Tick");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceCrushingBlowActive += Time.deltaTime;
        foreach (var item in pets)
        {
            item.Update();
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        attackCooldown = 0;
        hasCrushingBlow = true;
        timeSinceCrushingBlowActive = 0;
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW) || myStats.wLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        MyBuffManager.Add("InvisibleBuff", new UntargetableBuff(1f, MyBuffManager, WSkill().basic.name));
        pets.Add(new WarriorTrickster(this, myStats.currentHealth, myStats.AD, myStats.attackSpeed, myStats.spellBlock, myStats.armor, 3.25f, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats)));
        hasClone = true;
        yield return new WaitForSeconds(3.25f);
        hasClone = false;
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), SkillDamageType.Spell), ESkill().basic.name);
        MyBuffManager.Add("AttackSpeed", new AttackSpeedBuff(5f, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), "NimbusStrike"));
        if (hasClone)
        {
            UpdateAbilityTotalDamage(ref eSum, 2, new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), SkillDamageType.Spell), ESkill().basic.name);
            //need a way to add buff to pet
        }
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        StopCoroutine(PStackExpired());
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if (pStack < 10) pStack++;
        myStats.buffManager.buffs.Remove("StoneSkin");
        float armorValue = (5 + 4 / 17 * (myStats.level - 1));
        myStats.buffManager.buffs.Add("StoneSkin", new ArmorBuff(5f, myStats.buffManager, "StoneSkin", armorValue + armorValue * 0.5f * pStack, "StoneSkin"));
        //need to add bonus regen
        StartCoroutine(PStackExpired());

        if (timeSinceCrushingBlowActive < 5 && hasCrushingBlow)
        {
            AutoAttack(new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.Phyiscal));
            TargetBuffManager.Add("ArmorReduction", new ArmorReductionBuff(3f, TargetBuffManager, QSkill().basic.name, QSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), "CrushingBlow"));
            hasCrushingBlow = false;
            if (hasClone)
            {
                AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.Phyiscal));
                TargetBuffManager.Add("ArmorReduction", new ArmorReductionBuff(3f, TargetBuffManager, QSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * QSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), "CrushingBlow"));
            }
        }
    }

    private IEnumerator PStackExpired()
    {
        yield return new WaitForSeconds(5f);
        pStack = 0;
        myStats.buffManager.buffs.Remove("StoneSkin");
    }
}