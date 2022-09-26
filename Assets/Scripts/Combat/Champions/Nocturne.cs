using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Nocturne : ChampionCombat
{
    public float passiveNocturneReady;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "E", "W", "A" };

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
        checksR.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

        checkTakeDamageAbility.Add(new CheckSpellShield(this));

        qKeys.Add("Physical damage");
        qKeys.Add("Bonus Attack Damage");
        wKeys.Add("Bonus Attack Speed");
        eKeys.Add("Magic Damage per Tick");
        eKeys.Add("Disable Duration");
        rKeys.Add("Physical Damage");

        myStats.attackSpeed += myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats) * myStats.baseAttackSpeed; //probably incorrect calculation to give attackSpeed

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        passiveNocturneReady += Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.buffManager.buffs.Add("BonusAD", new AttackDamageBuff(5, myStats.buffManager, myStats.qSkill[0].basic.name, (int)myStats.qSkill[0].UseSkill(4, qKeys[2], myStats, targetStats), "BonusAD"));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("SpellShield", new SpellShieldBuff(1.5f, myStats.buffManager, "SpellShield"));
        myStats.buffManager.buffs.Add("AttackSpeed", new AttackSpeedBuff(5, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), "AttackSpeed"));     //this should be given after first buff expires
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.buffManager.buffs.Add("FearBuff", new FleeBuff(myStats.eSkill[0].UseSkill(4, eKeys[1], myStats, targetStats), targetStats.buffManager, "FearBuff"));
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;
        myStats.buffManager.buffs.Add("NearSight", new NearsightBuff(6, targetStats.buffManager, "NearSight"));
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime + 0.25f));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if(passiveNocturneReady < 13)
		{
            AutoAttack();
            passiveNocturneReady += 3;
        }
		else
		{
			AutoAttack(1.2f); //yet to implement crit
            UpdateTotalHeal(ref hSum, 12 + myStats.level + (myStats.AP * 0.3f), "Heal");
		}
	}
}