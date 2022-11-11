using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Azir : ChampionCombat
{
    public static float[] SandSoldierFlatDamageByLevel = { 50, 52, 54, 56, 58, 60, 62, 65, 70, 75, 80, 90, 100, 110, 120, 130, 140, 150 };
    public static float[] SandSoldierRechargeBySkillLevel = { 9f, 8.25f, 7.5f, 6.75f, 6f };
    public int SoldierCount = 0;
    public int SoldierStack = 2;
    public float SoldierRecharge;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "Q", "E", "A" };

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
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        wKeys.Add("Bonus Attack Speed");
        eKeys.Add("Magic Damage");
        eKeys.Add("Shield Strength");
        rKeys.Add("Magic Damage");

        myStats.attackSpeed += myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * myStats.baseAttackSpeed;

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        if (SoldierStack < 2)
        {
            SoldierRecharge += Time.fixedDeltaTime;
            if (SoldierRecharge >= SandSoldierRechargeBySkillLevel[myStats.wLevel])
            {
                SoldierStack++;
                SoldierRecharge = 0;
            }
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (SoldierCount == 0 && myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;
        if (SoldierStack == 0) yield break;
        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        SoldierStack--;
        StartCoroutine(SummonSoldier());
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        MyBuffManager.shields.Add(myStats.eSkill[0].basic.name, new ShieldBuff(1.5f, myStats.buffManager, myStats.eSkill[0].basic.name, myStats.eSkill[0].UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), myStats.eSkill[0].basic.name));
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        if(UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable) != float.MinValue)
        {
            TargetBuffManager.buffs.Add("Airborne", new AirborneBuff(1f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        }
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        if(UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]) != float.MinValue)
        {
            TargetBuffManager.buffs.Add("Airborne", new AirborneBuff(1f, myStats.buffManager, myStats.rSkill[0].basic.name));
        }
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (SoldierCount == 0) AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal, SkillComponentTypes.OnHit | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable));
        else
        {
            float damage = SandSoldierFlatDamageByLevel[myStats.level - 1] + (myStats.AP * 0.55f);
            UpdateTotalDamage(ref wSum, 1, new Damage(damage + ((SoldierCount - 1) * damage * 0.25f), SkillDamageType.Spell), "Arise!");
        }
    }

    public IEnumerator SummonSoldier()
    {
        SoldierCount++;
        if (SoldierCount == 3) myStats.buffManager.buffs.Add(myStats.wSkill[0].basic.name, new AttackSpeedBuff(5f, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), myStats.wSkill[0].basic.name));
        yield return new WaitForSeconds(5f);
        SoldierCount--;
    }
}