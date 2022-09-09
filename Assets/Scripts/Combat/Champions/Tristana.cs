using System.Collections;
using Simulator.Combat;

public class Tristana : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "W", "E", "Q", "A", "R" };

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

        targetCombat.checksQ.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksW.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksE.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksR.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksA.Add(new CheckIfStunned(targetCombat));

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("AmbushAS", new AttackSpeedBuff(5, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].selfEffects.ASIncreasePercent[4], "AmbushAS"));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        CheckExplosiveCharge();
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(attackCooldown));
        targetStats.buffManager.buffs.Add("Explosive Charge", new ExplosiveChargeBuff(4, targetStats.buffManager, myStats.eSkill[0].basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        yield return StartCoroutine(base.ExecuteR());
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.75f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        CheckExplosiveCharge();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
        CheckExplosiveCharge();
    }

    private void CheckExplosiveCharge()
    {
        if (myStats.buffManager.buffs.TryGetValue("Explosive Charge", out Buff explosiveCharge))
        {
            explosiveCharge.value++;
            if (explosiveCharge.value == 4)
            {
                myStats.wCD = 0;
                explosiveCharge.Kill();
            }
        }
    }
}
