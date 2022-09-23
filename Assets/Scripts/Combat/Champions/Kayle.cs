using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Kayle : ChampionCombat
{

    private int pStack = 0;
    private bool isExalted;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "W", "E", "R", "A" };

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
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        autoattackcheck = new KayleAACheck(this);
        checkTakeDamageAA.Add(new CheckKayleR(this));
        checkTakeDamageAbility.Add(new CheckKayleR(this));

        qKeys.Add("Magic Damage");
        wKeys.Add("Heal");
        eKeys.Add("Passive Damage");
        eKeys.Add("Bonus Magic Damage");
        rKeys.Add("Invulnerability Duration");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.buffManager.buffs.Add("ArmorReduction", new ArmorReductionBuff(4, targetStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats), "ArmorReduction"));
        myStats.buffManager.buffs.Add("MRReduction", new MagicResistanceReductionBuff(4, targetStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats), "MRReduction"));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateTotalHeal(ref wSum, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), "Heal");
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
        attackCooldown = 0;
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.TryAdd("Invulnerable", new UntargetableBuff(myStats.rSkill[0].UseSkill(2, myStats.rSkill[0].basic.name, myStats, targetStats), myStats.buffManager, "Invulnerable"));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        StopCoroutine(PStackExpired());
        AutoAttack();
        if (pStack < 5) pStack++;
        if (pStack == 5) isExalted = true;
        myStats.buffManager.buffs.Remove("DivineAscent");
        myStats.buffManager.buffs.Add("DivineAscent", new AttackSpeedBuff(5f, myStats.buffManager, "DivineAscent", pStack * 0.06f, "DivineAscent"));
        StartCoroutine(PStackExpired());
    }

    public IEnumerator PStackExpired()
	{
        yield return new WaitForSeconds(5f);
        pStack = 0;
        myStats.buffManager.buffs.Remove("DivineAscent");
    }
    public void DivineAscentPassive()
	{
        if(isExalted && myStats.level > 11)
		{

		}
	}
}