using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Trundle : ChampionCombat
{
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

        qKeys.Add("Bonus Physical Damage");
        qKeys.Add("Bonus Attack Damage");
        qKeys.Add("Attack Damage Reduction");
        wKeys.Add("Bonus Attack Speed");
        rKeys.Add("Total Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;
        if(myStats.buffManager.buffs.ContainsKey("Chomp")) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateTotalDamage(ref qSum, 0, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), QSkill().basic.name);
        MyBuffManager.Add("Chomp", new ChompBuff(7, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats)));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
        attackCooldown = 0;
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), WSkill().basic.name);
        MyBuffManager.Add("AttackSpeedBuff", new AttackSpeedBuff(8, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), "AttackSpeedBuff"));
        MyBuffManager.Add("IncreasedHeal", new GrievousWoundsBuff(8, myStats.buffManager, myStats.wSkill[0].basic.name, -25f, "IncreasedHeal"));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        TargetBuffManager.Add("KnockBackBuff", new AirborneBuff(0.1f, targetStats.buffManager, "KnockBackBuff"));
        UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), ESkill().basic.name);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        if(UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], damageModifier: 0.5f, skillComponentTypes: (SkillComponentTypes)2240) != float.MinValue)
        {
            UpdateTotalHeal(ref rSum, myStats.rSkill[0], myStats.rLevel, rKeys[0], healModifier: 0.5f);
            StartCoroutine(Subjugate());
            MyBuffManager.Add("BonusArmor", new ArmorBuff(8, myStats.buffManager, myStats.rSkill[0].basic.name, (float)0.4 * targetStats.armor, "BonusArmor"));
            TargetBuffManager.Add("ArmorReduction", new ArmorReductionBuff(8, targetStats.buffManager, myStats.qSkill[0].basic.name, (float)0.4 * targetStats.armor, "ArmorReduction"));
            MyBuffManager.Add("BonusMR", new MagicResistanceBuff(8, myStats.buffManager, myStats.qSkill[0].basic.name, (int)(0.4 * targetStats.spellBlock), "BonusMR"));
            TargetBuffManager.Add("MRReduction", new MagicResistanceReductionBuff(8, myStats.buffManager, myStats.qSkill[0].basic.name, (float)0.4 * targetStats.spellBlock, "MRReduction"));
        }
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if(UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Trundle's Auto Attack") != float.MinValue)
        {
            if (myStats.buffManager.buffs.TryGetValue("Chomp", out Buff value))
            {
                if (UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)34560) != float.MinValue)
                    MyBuffManager.Add("BonusAD", new AttackDamageBuff(5, myStats.buffManager, myStats.qSkill[0].basic.name, (int)myStats.qSkill[0].UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), "BonusAD"));
                if (UpdateTotalDamage(ref qSum, 0, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)128), QSkill().basic.name) != float.MinValue)
                    TargetBuffManager.Add("ADReduction", new AttackDamageReductionBuff(5, targetStats.buffManager, myStats.qSkill[0].basic.name, (int)myStats.qSkill[0].UseSkill(myStats.qLevel, qKeys[2], myStats, targetStats), "ADReduction"));
                myStats.buffManager.buffs.Remove("Chomp");
            }
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public IEnumerator Subjugate()
    {
        yield return new WaitForSeconds(1);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], damageModifier: 0.125f, skillComponentTypes: (SkillComponentTypes)64);
        UpdateTotalHeal(ref rSum, myStats.rSkill[0], myStats.rLevel, rKeys[0], healModifier: 0.125f);
        yield return new WaitForSeconds(1);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], damageModifier: 0.125f, skillComponentTypes: (SkillComponentTypes)64);
        UpdateTotalHeal(ref rSum, myStats.rSkill[0], myStats.rLevel, rKeys[0], healModifier: 0.125f);
        yield return new WaitForSeconds(1);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], damageModifier: 0.125f, skillComponentTypes: (SkillComponentTypes)64);
        UpdateTotalHeal(ref rSum, myStats.rSkill[0], myStats.rLevel, rKeys[0], healModifier: 0.125f);
        yield return new WaitForSeconds(1);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], damageModifier: 0.125f, skillComponentTypes: (SkillComponentTypes)64);
        UpdateTotalHeal(ref rSum, myStats.rSkill[0], myStats.rLevel, rKeys[0], healModifier: 0.125f);
    }
}