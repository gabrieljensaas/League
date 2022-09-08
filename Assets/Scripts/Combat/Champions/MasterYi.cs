using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class MasterYi : ChampionCombat
{
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
        checkTakeDamageAbility.Add(new CheckIfTargetable(this));
        checkTakeDamageAbility.Add(new CheckDamageReductionPercent(this));
        checkTakeDamageAA.Add(new CheckIfTargetable(this));
        checkTakeDamageAA.Add(new CheckDamageReductionPercent(this));
        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;
        if (myStats.buffManager.HasImmobilize) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(0.858f, myStats.buffManager, myStats.qSkill[0].basic.name));
        myStats.buffManager.buffs.Add("UnableToAct", new UnableToActBuff(0.858f, myStats.buffManager, myStats.qSkill[0].basic.name));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        if (myStats.buffManager.buffs.TryGetValue("WujuStyle", out Buff value)) value.paused = true;
        if (myStats.buffManager.buffs.TryGetValue("Highlander", out Buff highlander)) highlander.paused = true;
        yield return new WaitForSeconds(0.231f);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys, 0.25f);

        yield return new WaitForSeconds(0.231f);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys, 0.25f);

        yield return new WaitForSeconds(0.231f);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys, 0.25f);

        yield return new WaitForSeconds(0.165f);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys);
        if (myStats.buffManager.buffs.TryGetValue("WujuStyle", out Buff wuju)) wuju.paused = false;
        if (myStats.buffManager.buffs.TryGetValue("Highlander", out Buff highland)) highland.paused = false;
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, "Meditate"));
        myStats.buffManager.buffs.Add("DamageReductionPercent", new DamageReductionPercentBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, 90));
        if (myStats.buffManager.buffs.TryGetValue("WujuStyle", out Buff value)) value.paused = true;
        if (myStats.buffManager.buffs.TryGetValue("Highlander", out Buff highlander)) highlander.paused = true;
        StartCoroutine(Meditate());
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        attackCooldown = 0f;
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        myStats.buffManager.buffs.Add("WujuStyle", new WujuStyleBuff(5, myStats.buffManager, myStats.eSkill[0].basic.name, 50));  //when the bonus ad and skill level comes do the calculation
    }

    private IEnumerator Meditate()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys);
        if (myStats.buffManager.buffs.TryGetValue("DamageReductionPercent", out Buff value))
        {
            value.value = Constants.MasterYiWDamageReductionPercents[4];          //when skill level is done change index to variable
        }
        //pause wuju style and highlander durations
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys);
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
            myStats.buffManager.buffs.Add("DoubleStrike", new DoubleStrikeBuff(4, myStats.buffManager, "Double Strike"));
        }
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys);

        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys);
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
            myStats.buffManager.buffs.Add("DoubleStrike", new DoubleStrikeBuff(4, myStats.buffManager, "Double Strike"));
        }
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys);

        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys);
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
            myStats.buffManager.buffs.Add("DoubleStrike", new DoubleStrikeBuff(4, myStats.buffManager, "Double Strike"));
        }
        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys);

        yield return new WaitForSeconds(0.5f);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys);
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys);
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
            myStats.buffManager.buffs.Add("DoubleStrike", new DoubleStrikeBuff(4, myStats.buffManager, "Double Strike"));
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