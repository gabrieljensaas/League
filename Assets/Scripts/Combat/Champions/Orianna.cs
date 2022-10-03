using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Orianna : ChampionCombat
{
    public int passiveStack;
    public static float ClockworkWindUpDamageByLevel(int level, int stack)
    {
        if (stack < 2)
        {
            return level switch
            {
                < 4 => 10,
                < 7 => 18,
                < 10 => 26,
                < 13 => 34,
                < 16 => 42,
                _ => 50,
            };
        }
        else
        {
            return level switch
            {
                < 4 => 14,
                < 7 => 25.2f,
                < 10 => 36.4f,
                < 13 => 47.6f,
                < 16 => 58.8f,
                _ => 70,
            };
        }

    }


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
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Bonus Resistances"); //not yet implemented
        eKeys.Add("Magic Damage");
        eKeys.Add("Shield Strength");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), 4, qKeys[0]);
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), 4, wKeys[0]);
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), 4, eKeys[1]);
        //need help to add resistance when ball is with orianna
        MyBuffManager.Add(ESkill().basic.name, new ShieldBuff(2.5f, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(4, eKeys[2], myStats, targetStats), ESkill().basic.name));
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), 2, rKeys[0]);
        MyBuffManager.Add(RSkill().basic.name, new StunBuff(0.75f, TargetBuffManager, RSkill().basic.name));
        MyBuffManager.Add("KnockOff", new AirborneBuff(0.1f, TargetBuffManager, RSkill().basic.name));
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        StopCoroutine(PStackExpired());
        if (passiveStack == 1)
        {
            AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage(ClockworkWindUpDamageByLevel(myStats.level, passiveStack), SkillDamageType.Spell), myStats.passiveSkill.name);
            passiveStack++;
        }
        else if (passiveStack == 2)
        {
            AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage(ClockworkWindUpDamageByLevel(myStats.level, passiveStack), SkillDamageType.Spell), myStats.passiveSkill.name);
        }
        StartCoroutine(PStackExpired());
    }

    private IEnumerator PStackExpired()
    {
        yield return new WaitForSeconds(4f);
        passiveStack = 0;
    }
}