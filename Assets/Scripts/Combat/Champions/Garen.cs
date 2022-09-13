using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Garen : ChampionCombat
{
    public static float[] GarenEDamageByLevelTable = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 8.25f, 8.5f, 8.75f, 9, 9.25f, 9.5f, 9.75f, 10f, 10.25f };

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "Q", "A", "E" };

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
        checksA.Add(new CheckIfCantAA(this));
        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        autoattackcheck = new GarenAACheck(this);
        checkTakeDamageAbility.Add(new CheckDamageReductionPercent(this));
        checkTakeDamageAA.Add(new CheckDamageReductionPercent(this));
        checkTakeDamageAbility.Add(new CheckShield(this));
        checkTakeDamageAA.Add(new CheckShield(this));
        checksR.Add(new CheckIfExecutes(this, "R"));

        qKeys.Add("Bonus Physical Damage");
        wKeys.Add("Duration");
        wKeys.Add("Shield Strength");
        eKeys.Add("Increased Damage Per Spin");
        rKeys.Add("True Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("DecisiveStrike", new DecisiveStrikeBuff(4.5f, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats)));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("DamageReductionPercent", new DamageReductionPercentBuff(myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), myStats.buffManager, myStats.wSkill[0].basic.name, 30));
        myStats.buffManager.shields.Add(myStats.wSkill[0].basic.name, new ShieldBuff(0.75f, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[1], myStats, targetStats), myStats.wSkill[0].basic.name));
        myStats.buffManager.buffs.Add("Tenacity", new TenacityBuff(0.75f, myStats.buffManager, myStats.wSkill[0].basic.name, 60, myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        simulationManager.ShowText($"Garen Used Judgment!");
        myStats.buffManager.buffs.Add("CantAA", new CantAABuff(3f, myStats.buffManager, myStats.eSkill[0].basic.name));
        StartCoroutine(GarenE(0, 0));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];

        StopCoroutine("GarenE");          //if 2 GarenE coroutine exists this could leat to some bugs
        if (myStats.buffManager.buffs.ContainsKey("CantAA"))
        {
            myStats.buffManager.buffs.Remove("CantAA");
        }
    }

    private IEnumerator GarenE(float seconds, int spinCount)
    {
        yield return new WaitForSeconds(seconds);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        spinCount++;
        if (spinCount >= 6 && targetStats.buffManager.buffs.ContainsKey("Judgment"))
        {
            targetStats.buffManager.buffs["Judgment"].duration = 6;
        }
        else if (spinCount >= 6)
        {
            targetStats.buffManager.buffs.Add("Judgment", new ArmorReductionBuff(6, targetStats.buffManager, "Judgment", 25, "Judgment"));
        }
        if (spinCount > 6)
        {
            yield break;
        }
        StartCoroutine(GarenE(3f / 7f, spinCount));
    }
}
