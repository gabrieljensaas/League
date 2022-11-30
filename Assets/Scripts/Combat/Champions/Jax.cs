using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Jax : ChampionCombat
{
    private int pStack = 0;
    private int rStack = 0;

    public static float RelentlessAssaultAS(int level, int stack)
    {
        return level switch
        {
            < 4 => 3.5f * stack,
            < 7 => 5f * stack,
            < 10 => 6.5f * stack,
            < 13 => 9.5f * stack,
            _ => 11f * stack
        };
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "W", "A" };

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
        checksQ.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

        checkTakeDamage.Add(new CheckCounterStrike(this));

        qKeys.Add("Physical Damage");

        wKeys.Add("Bonus Magic Damage");

        eKeys.Add("Minimum Physical Damage");
        eKeys.Add("Maximum Physical Damage");

        rKeys.Add("Bonus Magic Damage");
        rKeys.Add("Bonus Armor");
        rKeys.Add("Bonus Magic Resistance");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        StopCoroutine(PStackExpired());
        if (UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Jax's Auto Attack") != float.MinValue)
		{
            if (TargetBuffManager.buffs.TryGetValue("EmpowerBuff", out Buff buff))
            {
                UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)24944);
                buff.Kill();
            }
        }
        if (pStack < 8) pStack++;
        MyBuffManager.buffs.Remove("RelentlessAssault");
        MyBuffManager.Add("RelentlessAssault", new AttackSpeedBuff(2.5f, MyBuffManager, "RelentlessAssault", RelentlessAssaultAS(myStats.level, pStack), "RelentlessAssault"));
        StartCoroutine(PStackExpired());

        if (rStack < 3)
            rStack++;
        else
        {
            rStack = 0;
            UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)928);
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);

    }
    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)34946);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), WSkill().basic.name);
        MyBuffManager.Add("EmpowerBuff", new EmpowerBuff(10, MyBuffManager, WSkill().name));
        attackCooldown = 0;
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        MyBuffManager.Add("CounterStrikeBuff", new CounterStrikeBuff(2, MyBuffManager, ESkill().name));
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        MyBuffManager.Add("BonusArmor", new ArmorBuff(8, MyBuffManager, RSkill().name, (int)RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats), "BonusArmor"));
        MyBuffManager.Add("BonusMR", new MagicResistanceBuff(8, MyBuffManager, RSkill().name, (int)RSkill().UseSkill(myStats.rLevel, rKeys[2], myStats, targetStats), "BonusMR"));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 4);
    }

    private IEnumerator PStackExpired()
    {
        yield return new WaitForSeconds(2.5f);
        pStack--;
        MyBuffManager.buffs.Remove("RelentlessAssault");
        if (pStack > 0) MyBuffManager.Add("RelentlessAssault", new AttackSpeedBuff(2.5f, MyBuffManager, "RelentlessAssault", RelentlessAssaultAS(myStats.level, pStack), "RelentlessAssault"));
    }
}