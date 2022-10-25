using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Diana : ChampionCombat
{
    public static float MoonsilverBladeAS(int level)
    {
        return level switch
        {
            < 3 => 0.15f,
            < 6 => 0.1917f,
            < 9 => 0.275f,
            < 12 => 0.3167f,
            < 15 => 0.3583f,
            _ => 0.4f
        };
    }

    public static float MoonsilverBladeCleave(int level)
    {
        return level switch
        {
            < 7 => 20 + 5 * (level - 1),
            < 12 => 45 + 10 * (level - 6),
            < 16 => 95 + 15 * (level - 11),
            _ => 155 + 25 * (level - 15)
        };
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "E", "W", "R", "A" };

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

        checkTakeDamagePostMitigation.Add(new CheckShield(this));
        checkTakeDamagePostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        wKeys.Add("Shield Strength");
        wKeys.Add("Total Shield Strength");
        wKeys.Add("Magic Damage per Orb");
        wKeys.Add("Total Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Slow");
        rKeys.Add("Magic Damage");
        rKeys.Add("Bonus Damage Per Champion");
        rKeys.Add("Total Damage Vs. 5 Champions");

        base.UpdatePriorityAndChecks();

        myStats.buffManager.buffs.Add("MoonsilverAS", new AttackSpeedBuff(999f, myStats.buffManager, "Moonsilver Blade", MoonsilverBladeAS(myStats.level), "MoonsilverAS"));
    }

    public override IEnumerator ExecuteA()
    {
        yield return base.ExecuteA();
        MoonsilverBladeAA();
    }

    public override IEnumerator ExecuteQ()
    {
        yield return base.ExecuteQ();
        targetStats.buffManager.buffs.Add("Moonlight", new MoonlightBuff(3, targetStats.buffManager, myStats.qSkill[0].name));
        MoonsilverBladeBonusAS();
    }

    public override IEnumerator ExecuteW()
    {
        yield return base.ExecuteW();
        myStats.buffManager.buffs.Add("PaleCascade", new ShieldBuff(5, myStats.buffManager, myStats.wSkill[0].name, myStats.wSkill[0].UseSkill(4, wKeys[1], myStats, targetStats), "PaleCascade"));
        MoonsilverBladeBonusAS();
    }

    public override IEnumerator ExecuteE()
    {
        yield return base.ExecuteE();
        if (targetStats.buffManager.buffs.TryGetValue("Moonlight", out Buff buff))
        {
            buff.Kill();
            myStats.eCD = 0.5f;
        }
        MoonsilverBladeBonusAS();
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.25f, targetStats.buffManager, myStats.rSkill[0].name));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];

        yield return new WaitForSeconds(1);
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[1], 2, rKeys[0]);
        MoonsilverBladeBonusAS();
    }

    private void MoonsilverBladeAA()
    {
        if (myStats.buffManager.buffs.TryGetValue("MoonsilverBlade", out Buff buff))
        {
            if (++buff.value == 2)
            {
                buff.Kill();
                UpdateTotalDamage(ref pSum, 4, new Damage(MoonsilverBladeCleave(myStats.level) + (myStats.AP * 0.5f), SkillDamageType.Spell), myStats.passiveSkill.skillName);
            }
        }
        else
            myStats.buffManager.buffs.Add("MoonsilverBlade", new MoonsilverBladeBuff(3.5f, myStats.buffManager, myStats.passiveSkill.skillName));
    }

    private void MoonsilverBladeBonusAS()
    {
        StopCoroutine(ReturnToNormalMoonsilverAS());
        if (myStats.buffManager.buffs.TryGetValue("MoonsilverAS", out Buff AS))
            AS.Kill();
        else if (myStats.buffManager.buffs.TryGetValue("MoonsilverBonusAS", out Buff bonusAS))
            bonusAS.duration = 3;

        myStats.buffManager.buffs.Add("MoonsilverBonusAS", new AttackSpeedBuff(3, myStats.buffManager, "Moonsilver Blade Bonus AS", MoonsilverBladeAS(myStats.level) * 3, "MoonsilverBonusAS"));
        StartCoroutine(ReturnToNormalMoonsilverAS());
    }

    private IEnumerator ReturnToNormalMoonsilverAS()
    {
        yield return new WaitForSeconds(3);
        myStats.buffManager.buffs.Add("MoonsilverAS", new AttackSpeedBuff(999f, myStats.buffManager, "Moonsilver Blade", MoonsilverBladeAS(myStats.level), "MoonsilverAS"));
    }
}
