using Simulator.Combat;
using System.Collections;

public class Vayne : ChampionCombat
{
    CheckIfEmpoweredTumble empoweredTumbleCheck;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "A", "" };

        checksQ.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckCD(this, "Q"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        checksQ.Add(new CheckIfAlreadyTumbled(this));
        autoattackcheck = new VayneAACheck(this);
        empoweredTumbleCheck = new CheckIfEmpoweredTumble(this);
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksQ.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksW.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksE.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksR.Add(new CheckIfStunned(targetCombat));
        targetCombat.checksA.Add(new CheckIfStunned(targetCombat));

        qKeys.Add("Bonus Physical Damage");
        wKeys.Add("Bonus True Damage");
        wKeys.Add("Minimum True Damage");
        eKeys.Add("Total Damage");
        rKeys.Add("Duration");
        rKeys.Add("Bonus Attack Damage");
        rKeys.Add("Tumble Cooldown Reduction");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;
        if (myStats.buffManager.HasImmobilize) yield break;
        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Tumble" , new TumbleBuff(7, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats)));
        if (empoweredTumbleCheck.Control())
        {
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4] * myStats.rSkill[0].UseSkill(2, rKeys[2], myStats, targetStats);
            myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(1, myStats.buffManager, myStats.qSkill[0].basic.name));
        }
        else
        {
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        if (myStats.buffManager.buffs.TryGetValue("Untargetable", out Buff value))
        {
            value.Kill();
        }
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(1.5f, targetStats.buffManager, myStats.eSkill[0].basic.name));
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        float duration = myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
        myStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new AttackDamageBuff(duration, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[1], myStats, targetStats), myStats.rSkill[0].basic.name));
        myStats.buffManager.buffs.Add("EmpoweredTumble", new EmpoweredTumbleBuff(duration, myStats.buffManager, myStats.rSkill[0].basic.name));
    }
}