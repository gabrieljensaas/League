using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class FiddleSticks : ChampionCombat
{
    private float bountifulHarvestDamage;

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

        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));

        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Fear Duration");
        qKeys.Add("Magic Damage");
        qKeys.Add("Minimum Damage");
        qKeys.Add("Fear Duration");
        qKeys.Add("Increased Magic Damage");
        qKeys.Add("Increased Minimum Damage");

        wKeys.Add("Magic Damage per Tick");
        wKeys.Add("Last Tick of Damage");
        wKeys.Add("Total Magic Damage");
        wKeys.Add("Champion Heal Percentage");
        wKeys.Add("Total Heal per Champion");

        eKeys.Add("Magic Damage");
        eKeys.Add("Slow");

        rKeys.Add("Magic Damage per Tick");
        rKeys.Add("Total Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));

        float damage = (QSkill().UseSkill(4, qKeys[1], myStats, targetStats) > QSkill().UseSkill(4, qKeys[2], myStats, targetStats)) ? QSkill().UseSkill(4, qKeys[1], myStats, targetStats) : QSkill().UseSkill(4, qKeys[2], myStats, targetStats);
        UpdateAbilityTotalDamage(ref qSum, 0, new Damage(damage, SkillDamageType.Spell), QSkill().name);
        TargetBuffManager.Add("Flee", new FleeBuff(QSkill().UseSkill(4, qKeys[0], myStats, targetStats), TargetBuffManager, QSkill().basic.name));
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));

        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(2, myStats.buffManager, WSkill().basic.name, WSkill().basic.name));
        StartCoroutine(BountifulHarvest());
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        yield return base.ExecuteE();
        TargetBuffManager.Add("Silence", new SilenceBuff(1.25f, TargetBuffManager, ESkill().name));
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(1.5f, myStats.buffManager, RSkill().basic.name, RSkill().basic.name));
        myStats.rCD = RSkill().basic.coolDown[2];

    }

    public override void StopChanneling(string uniqueKey)
    {
        if (uniqueKey == RSkill().basic.name)
            StartCoroutine(Crowstorm());
        else if (uniqueKey == WSkill().basic.name)
        {
            UpdateTotalHeal(ref hSum, WSkill().UseSkill(4, wKeys[3], myStats, targetStats) * bountifulHarvestDamage, WSkill().name);
            bountifulHarvestDamage = 0;
        }
    }

    private IEnumerator BountifulHarvest()
    {
        for (int i = 0; i < 7; i++)
        {
            bountifulHarvestDamage += UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), 4, wKeys[0]);
            yield return new WaitForSeconds(0.25f);
        }
        bountifulHarvestDamage += UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), 4, wKeys[1]);
    }

    private IEnumerator Crowstorm()
    {
        for (int i = 0; i < 20; i++)
        {
            UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), 2, rKeys[0]);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
