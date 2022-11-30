using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class MasterYi : ChampionCombat
{
    public static float[] MasterYiWDamageReductionPercents = { 45f, 47.5f, 50f, 52.5f, 55f };

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "W", "A" };

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
        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        checksQ.Add(new CheckIfUnableToAct(this));
        checksW.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));
        autoattackcheck = new MasterYiAACheck(this);
        checksE.Add(new CheckIfWujuStyle(this));
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checkTakeDamage.Add(new CheckIfTargetable(this));
        checkTakeDamage.Add(new CheckDamageReductionPercent(this));
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksQ.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Physical Damage");
        qKeys.Add("Reduced Damage per hit");
        wKeys.Add("Minimum Healing Per Half Second");
        wKeys.Add("Damage Reduction");
        eKeys.Add("True Damage");
        rKeys.Add("Bonus Attack Speed");

        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }
    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Master Yi's Auto Attack") != float.MinValue)
        {
            myStats.qCD--;
            if (MyBuffManager.buffs.TryGetValue("WujuStyle", out Buff buff))
			{
                UpdateTotalDamage(ref eSum, 2, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), SkillDamageType.True, skillComponentType: (SkillComponentTypes)1320), ESkill().basic.name);

            }
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        MyBuffManager.Add("Untargetable", new UntargetableBuff(0.858f, myStats.buffManager, myStats.qSkill[0].basic.name));
        MyBuffManager.Add("UnableToAct", new UnableToActBuff(0.858f, myStats.buffManager, myStats.qSkill[0].basic.name));
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
        if (myStats.buffManager.buffs.TryGetValue("WujuStyle", out Buff value)) value.paused = true;
        if (myStats.buffManager.buffs.TryGetValue("Highlander", out Buff highlander)) highlander.paused = true;
        yield return new WaitForSeconds(0.231f);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes:(SkillComponentTypes)22664);

        yield return new WaitForSeconds(0.231f);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)20616);

        yield return new WaitForSeconds(0.231f);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)20616);

        yield return new WaitForSeconds(0.165f);
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[1], skillComponentTypes: (SkillComponentTypes)20616);
        if (myStats.buffManager.buffs.TryGetValue("WujuStyle", out Buff wuju)) wuju.paused = false;
        if (myStats.buffManager.buffs.TryGetValue("Highlander", out Buff highland)) highland.paused = false;
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateTotalDamage(ref wSum, 1, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), WSkill().basic.name);
        MyBuffManager.Add("Channeling", new ChannelingBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, "Meditate"));
        MyBuffManager.Add("DamageReductionPercent", new DamageReductionPercentBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, 90));
        if (myStats.buffManager.buffs.TryGetValue("WujuStyle", out Buff value)) value.paused = true;
        if (myStats.buffManager.buffs.TryGetValue("Highlander", out Buff highlander)) highlander.paused = true;
        StartCoroutine(Meditate());
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
        attackCooldown = 0f;
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), ESkill().basic.name);

        MyBuffManager.Add("WujuStyle", new WujuStyleBuff(5, myStats.buffManager, myStats.eSkill[0].basic.name, myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats)));  //when the bonus ad and skill level comes do the calculation
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateTotalDamage(ref rSum, 3, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), RSkill().basic.name);
        MyBuffManager.Add(myStats.rSkill[0].basic.name, new AttackSpeedBuff(7, myStats.buffManager, myStats.rSkill[0].basic.name, myStats.rSkill[0].UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), myStats.rSkill[0].basic.name));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 4);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new AttackSpeedBuff(7, targetStats.buffManager, myStats.rSkill[0].basic.name, myStats.rSkill[0].SylasUseSkill(skillLevel, rKeys[0], targetStats, myStats), myStats.rSkill[0].basic.name));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
    }

    private IEnumerator Meditate()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * (1 + (myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth), myStats.wSkill[0].basic.name);
        if (myStats.buffManager.buffs.TryGetValue("DamageReductionPercent", out Buff value))
        {
            value.value = myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats);          //when skill level is done change index to variable
        }
        //pause wuju style and highlander durations
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * (1 + (myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth), myStats.wSkill[0].basic.name);
        if (myStats.buffManager.buffs.TryGetValue("DoubleStrike", out Buff val))
        {
            if (val.value < 3)
            {
                val.value++;
                val.duration = 4;
                simulationManager.ShowText($"{myStats.name} Gained a Stack of Double Strike!");
            }
        }
        else
        {
            MyBuffManager.Add("DoubleStrike", new DoubleStrikeBuff(4, myStats.buffManager, "Double Strike"));
        }
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * (1 + (myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth), myStats.wSkill[0].basic.name);

        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * (1 + (myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth), myStats.wSkill[0].basic.name);
        if (myStats.buffManager.buffs.TryGetValue("DoubleStrike", out Buff buff))
        {
            if (buff.value < 3)
            {
                buff.value++;
                buff.duration = 4;
                simulationManager.ShowText($"{myStats.name} Gained a Stack of Double Strike!");
            }
        }
        else
        {
            MyBuffManager.Add("DoubleStrike", new DoubleStrikeBuff(4, myStats.buffManager, "Double Strike"));
        }
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * (1 + (myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth), myStats.wSkill[0].basic.name);

        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * (1 + (myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth), myStats.wSkill[0].basic.name);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * (1 + (myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth), myStats.wSkill[0].basic.name);
        if (myStats.buffManager.buffs.TryGetValue("DoubleStrike", out Buff buf))
        {
            if (buf.value < 3)
            {
                buf.value++;
                buf.duration = 4;
                simulationManager.ShowText($"{myStats.name} Gained a Stack of Double Strike!");
            }
        }
        else
        {
            MyBuffManager.Add("DoubleStrike", new DoubleStrikeBuff(4, myStats.buffManager, "Double Strike"));
        }
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * (1 + (myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth), myStats.wSkill[0].basic.name);

        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * (1 + (myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth), myStats.wSkill[0].basic.name);
        if (myStats.buffManager.buffs.TryGetValue("DoubleStrike", out Buff doubleStrike))
        {
            if (doubleStrike.value < 3)
            {
                doubleStrike.value++;
                doubleStrike.duration = 4;
                simulationManager.ShowText($"{myStats.name} Gained a Stack of Double Strike!");
            }
        }
        else
        {
            MyBuffManager.Add("DoubleStrike", new DoubleStrikeBuff(4, myStats.buffManager, "Double Strike"));
        }
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
        if (myStats.buffManager.buffs.TryGetValue("DamageReductionPercent", out Buff value))
        {
            value.Kill();
        }
        if (myStats.buffManager.buffs.TryGetValue("WujuStyle", out Buff wuju)) wuju.paused = false;
        if (myStats.buffManager.buffs.TryGetValue("Highlander", out Buff highlander)) highlander.paused = false;
    }
}