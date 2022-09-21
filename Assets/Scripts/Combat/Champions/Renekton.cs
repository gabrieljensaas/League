using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Renekton : ChampionCombat
{
    private int furyPassive;
    private bool AngerMode = false;
	private bool eCast;
    private float timeSinceE;

	public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

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

        qKeys.Add("Physical Damage");
        qKeys.Add("Enhanced Damage");
        qKeys.Add("Champion Healing");
        qKeys.Add("Enhanced Champion Healing");
        wKeys.Add("Total Physical Damage");
        wKeys.Add("Total Physical Damage");
        eKeys.Add("Physical Damage");
        eKeys.Add("Total Physical Damage");
        eKeys.Add("Bonus Physical Damage");
        eKeys.Add("Armor Reduction");
        rKeys.Add("Bonus Health");          // 0
        rKeys.Add("Total Magic Damage");    // 1

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceE += Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if(AngerMode)
		{
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[1]);
            UpdateTotalHeal(ref hSum,myStats.qSkill[0].UseSkill(4, qKeys[4], myStats, targetStats), myStats.qSkill[0].basic.name);
        }
		else
		{
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
            UpdateTotalHeal(ref hSum, myStats.qSkill[0].UseSkill(4, qKeys[3], myStats, targetStats), myStats.qSkill[0].basic.name);
        }
        furyPassive += 10;
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));

        if (AngerMode)
        {
            UpdateAbilityTotalDamage(ref wSum, 0, myStats.wSkill[0], 4, wKeys[1]);
            myStats.buffManager.buffs.Add("EmpStunBuff", new StunBuff(1.5f, myStats.buffManager, myStats.wSkill[0].basic.name));
            //need help to implement shield break
        }
        else
        {
            UpdateAbilityTotalDamage(ref wSum, 0, myStats.wSkill[0], 4, wKeys[0]);
            myStats.buffManager.buffs.Add("EmpStunBuff", new StunBuff(0.75f, myStats.buffManager, myStats.wSkill[0].basic.name));
        }
        furyPassive += 10;
        attackCooldown = 0f;
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        if (!eCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            StartCoroutine(SliceDice());
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
            timeSinceE = 0;
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            if(AngerMode)
			{
                UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
                UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[2]);
                myStats.buffManager.buffs.Add("ArmorReduction", new ArmorReductionBuff(4f, myStats.buffManager, myStats.eSkill[0].basic.name, myStats.eSkill[0].UseSkill(4, qKeys[3], myStats, targetStats), "ArmorReduction"));
            }
			else
			{
                UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
            }

            StopCoroutine(SliceDice());
            myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
        }
        furyPassive += 10;
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        StartCoroutine(Dominus());
        UpdateAbilityTotalDamage(ref rSum, 2, myStats.rSkill[0], 4, rKeys[1]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
        furyPassive += 5;
        if(furyPassive >= 50)
		{
            AutoAttack();
            furyPassive -= 50;
            AngerMode = true;
		}
    }
    public IEnumerator SliceDice()
    {
        eCast = true;
        yield return new WaitForSeconds(4f);
        eCast = false;
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
    }

    public IEnumerator Dominus()
	{
        myStats.maxHealth = myStats.currentHealth + myStats.rSkill[0].UseSkill(2, rKeys[0], myStats ,targetStats);
        yield return new WaitForSeconds(15f);
        myStats.maxHealth = myStats.currentHealth - myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
    }
}