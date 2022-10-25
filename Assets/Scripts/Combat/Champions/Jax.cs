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
        checksA.Add(new CheckIfDisarmed(this));

        autoattackcheck = new JaxAACheck(this);
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
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if (pStack < 8) pStack++;
        myStats.buffManager.buffs.Remove("RelentlessAssault");
        myStats.buffManager.buffs.Add("RelentlessAssault", new AttackSpeedBuff(2.5f, myStats.buffManager, "RelentlessAssault", RelentlessAssaultAS(myStats.level, pStack), "RelentlessAssault"));
        StartCoroutine(PStackExpired());

        if (rStack < 3)
            rStack++;
        else
        {
            rStack = 0;
            UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("EmpowerBuff", new EmpowerBuff(10, myStats.buffManager, myStats.wSkill[0].name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("CounterStrikeBuff", new CounterStrikeBuff(10, myStats.buffManager, myStats.eSkill[0].name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("BonusArmor", new ArmorBuff(8, myStats.buffManager, myStats.rSkill[0].name, (int)myStats.rSkill[0].UseSkill(2, rKeys[1], myStats, targetStats), "BonusArmor"));
        myStats.buffManager.buffs.Add("BonusMR", new MagicResistanceBuff(8, myStats.buffManager, myStats.rSkill[0].name, (int)myStats.rSkill[0].UseSkill(2, rKeys[2], myStats, targetStats), "BonusMR"));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    private IEnumerator PStackExpired()
    {
        yield return new WaitForSeconds(2.5f);
        pStack--;
        myStats.buffManager.buffs.Remove("RelentlessAssault");
        if (pStack > 0) myStats.buffManager.buffs.Add("RelentlessAssault", new AttackSpeedBuff(2.5f, myStats.buffManager, "RelentlessAssault", RelentlessAssaultAS(myStats.level, pStack), "RelentlessAssault"));
    }
}