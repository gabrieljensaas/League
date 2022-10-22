using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Zilean : ChampionCombat
{
    private bool usedRewind;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "W", "R", "E", "A" };

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

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        checkTakeDamageAA.Add(new CheckForChronoshift(this));
        checkTakeDamageAbility.Add(new CheckForChronoshift(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Stun Duration");

        rKeys.Add("Heal");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if(usedRewind)
		{
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable);   
		}
        else
		{
            yield return new WaitForSeconds(3f);
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable);
        }

        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW) || myStats.wLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        usedRewind = true;
        myStats.qCD -= 10;
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if(myStats.PercentCurrentHealth < 0.3f)
		{
            if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

            yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime)); // need revision
            if(myStats.currentHealth == 0)
			{
                MyBuffManager.Add("Invulnerable", new UntargetableBuff(3f, MyBuffManager, RSkill().basic.name));
                MyBuffManager.Add("UnabletoAct", new UnableToActBuff(3f, MyBuffManager, RSkill().basic.name));
                HealHealth(RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats), RSkill().basic.name);
            }
            myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        }
    }
}
public class CheckForChronoshift : Check
{
    public CheckForChronoshift(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        return damage;
    }
}