using System.Collections;
using UnityEngine;
using Simulator.Combat;

public class Garen : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "Q", "A", "E" };
        checksQ.Add(new CheckIfCasting(this));
        targetCombat.checksQ.Add(new CheckIfSilenced(targetCombat));
        checksW.Add(new CheckIfCasting(this));
        targetCombat.checksW.Add(new CheckIfSilenced(targetCombat));
        checksE.Add(new CheckIfCasting(this));
        targetCombat.checksE.Add(new CheckIfSilenced(targetCombat));
        checksR.Add(new CheckIfCasting(this));
        targetCombat.checksR.Add(new CheckIfSilenced(targetCombat));
        checksA.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCantAA(this));
        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        autoattackcheck = new GarenAACheck(this);
        checkTakeDamage.Add(new CheckDamageReductionPercent(this));
        checkTakeDamageAA.Add(new CheckDamageReductionPercent(this));
        checkTakeDamage.Add(new CheckShield(this));
        checkTakeDamageAA.Add(new CheckShield(this));
        checksR.Add(new CheckIfExecutes(this, "R"));
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        simulationManager.ShowText($"Garen Used Judgment!");
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        myStats.buffManager.buffs.Add("CantAA", new CantAABuff(3f, myStats.buffManager, myStats.eSkill[0].basic.name));
        StartCoroutine(GarenE(0, 0));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 3, myStats.rSkill[0], 2);
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
        eSum += targetCombat.TakeDamage(myStats.eSkill[0].UseSkill(4, myStats, targetStats), myStats.eSkill[0].basic.name, SkillDamageType.Phyiscal);
        myUI.abilitySum[2].text = eSum.ToString();
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
