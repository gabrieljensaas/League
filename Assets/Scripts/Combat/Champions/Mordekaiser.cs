using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Mordekaiser : ChampionCombat
{
    private float shieldStored;
    public static float DarknessRiseDamage(int level) => 4.4f + (0.6f * level);
    public static float DarknessRiseTargetMaxHPPercentDamage(int level) => 0.01f + (0.04f / 17 * (level - 1));
    
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "A", "W" };

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

        checkTakeDamageAA.Add(new CheckMordekaiserIndestructible(this, this));
        checkTakeDamageAbility.Add(new CheckMordekaiserIndestructible(this, this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Damage Increase");
        wKeys.Add("Shield to Healing");
        eKeys.Add("Magic Penetration");
        eKeys.Add("Magic Damage");

        targetStats.spellBlock *= (100 - myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats)) * 0.01f;
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        Indestructible(AutoAttack());
        Indestructible(UpdateAbilityTotalDamage(ref pSum, 4, 0.4f * myStats.AP, myStats.passiveSkill.skillName, SkillDamageType.Spell));
        DarknessRiseStacks(myStats.passiveSkill.skillName);
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        Indestructible(UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]));
        Indestructible(UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[1])); //additional damage
        DarknessRiseStacks(myStats.qSkill[0].name);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    //decay is unspecified in fandom
    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.shields.Add(myStats.wSkill[0].basic.name, new ShieldBuff(5, myStats.buffManager, myStats.wSkill[0].basic.name, shieldStored, myStats.wSkill[0].basic.name));
        shieldStored = 0;
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];

        StartCoroutine(ConsumeShield());
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        Indestructible(UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]));
        DarknessRiseStacks(myStats.eSkill[0].name);
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.1f, targetStats.buffManager, myStats.eSkill[0].name)); //pull
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateTotalHeal(ref hSum, (targetStats.maxHealth * 0.1f), myStats.rSkill[0].name);
        StartCoroutine(StealStats());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    private void DarknessRiseStacks(string source)
    {
        if(myStats.buffManager.buffs.TryGetValue("DarknessRise", out Buff buff))
        {
            buff.duration = 4;
            if (buff.value < 3) buff.value++;
        }
        else
            myStats.buffManager.buffs.Add("DarknessRise", new DarknessRiseBuff(4, myStats.buffManager, source, this));
    }

    private IEnumerator StealStats()
    {
        myStats.AD += targetStats.AD * 0.1f;
        myStats.AP += targetStats.AP * 0.1f;
        myStats.attackSpeed += targetStats.attackSpeed * 0.1f;
        myStats.armor += targetStats.armor * 0.1f;
        myStats.spellBlock += targetStats.spellBlock * 0.1f;

        targetStats.AD *= 0.9f;
        targetStats.AP *= 0.9f;
        targetStats.attackSpeed *= 0.9f;
        targetStats.armor *= 0.9f;
        targetStats.spellBlock *= 0.9f;

        yield return new WaitForSeconds(7f);

        myStats.AD -= targetStats.AD * 0.1f;
        myStats.AP -= targetStats.AP * 0.1f;
        myStats.attackSpeed -= targetStats.attackSpeed * 0.1f;
        myStats.armor -= targetStats.armor * 0.1f;
        myStats.spellBlock -= targetStats.spellBlock * 0.1f;

        targetStats.AD /= 0.9f;
        targetStats.AP /= 0.9f;
        targetStats.attackSpeed /= 0.9f;
        targetStats.armor /= 0.9f;
        targetStats.spellBlock /= 0.9f;
    }

    //did not add decay because champion wil always be in combat anyways
    public void Indestructible(float damage)
    {
        shieldStored += damage;

        if (shieldStored > myStats.maxHealth * 0.3f) shieldStored = myStats.maxHealth * 0.3f;
    }

    private IEnumerator ConsumeShield()
    {
        yield return new WaitForSeconds(4);
        if(myStats.buffManager.buffs.TryGetValue(myStats.wSkill[0].basic.name, out Buff buff))
        {
            UpdateTotalHeal(ref hSum, buff.value * myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats) * 0.01f, myStats.wSkill[0].name); 
            buff.Kill();
        }
    }
}
