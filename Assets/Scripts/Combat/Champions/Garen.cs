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
        checkTakeDamage.Add(new CheckDamageReductionPercent(this));
        checkTakeDamagePostMitigation.Add(new CheckShield(this));
        checksR.Add(new CheckIfExecutes(this, "R"));

        qKeys.Add("Bonus Physical Damage");

        wKeys.Add("Duration");
        wKeys.Add("Shield Strength");
        
        eKeys.Add("Increased Damage Per Spin");
        
        rKeys.Add("True Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Garen's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        MyBuffManager.Add("DecisiveStrike", new DecisiveStrikeBuff(4.5f, MyBuffManager, QSkill().name, QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats)));
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        MyBuffManager.Add("DamageReductionPercent", new DamageReductionPercentBuff(WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), MyBuffManager, WSkill().basic.name, 30));
        MyBuffManager.shields.Add(WSkill().basic.name, new ShieldBuff(0.75f, MyBuffManager, WSkill().basic.name, WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), WSkill().basic.name));
        MyBuffManager.Add("Tenacity", new TenacityBuff(0.75f, MyBuffManager, WSkill().basic.name, 60, WSkill().basic.name));
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        simulationManager.ShowText($"Garen Used Judgment!");
        MyBuffManager.Add("CantAA", new CantAABuff(3f, MyBuffManager, ESkill().basic.name));
        StartCoroutine(GarenE(0, 0));
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)34944);
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 4);

        StopCoroutine("GarenE");          //if 2 GarenE coroutine exists this could leat to some bugs
        if (MyBuffManager.buffs.ContainsKey("CantAA"))
        {
            MyBuffManager.buffs.Remove("CantAA");
        }
    }

    private IEnumerator GarenE(float seconds, int spinCount)
    {
        yield return new WaitForSeconds(seconds);
        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes:(SkillComponentTypes)8192);
        spinCount++;
        if (spinCount >= 6 && TargetBuffManager.buffs.ContainsKey("Judgment"))
        {
            TargetBuffManager.buffs["Judgment"].duration = 6;
        }
        else if (spinCount >= 6)
        {
            TargetBuffManager.Add("Judgment", new ArmorReductionBuff(6, TargetBuffManager, "Judgment", 25, "Judgment"));
        }
        if (spinCount > 6)
        {
            yield break;
        }
        StartCoroutine(GarenE(3f / 7f, spinCount));
    }
}
