using Simulator.Combat;
using System.Collections;

public class Ashe : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "A", "W", "R", "" };

        checksQ.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckAsheQ(this));
        targetCombat.checksQ.Add(new CheckIfStunned(targetCombat));
        checksW.Add(new CheckIfCasting(this));
        targetCombat.checksW.Add(new CheckIfStunned(targetCombat));
        checksE.Add(new CheckIfCasting(this));
        targetCombat.checksE.Add(new CheckIfStunned(targetCombat));
        checksR.Add(new CheckIfCasting(this));
        targetCombat.checksR.Add(new CheckIfStunned(targetCombat));
        checksA.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        targetCombat.checksA.Add(new CheckIfStunned(targetCombat));
        autoattackcheck = new AsheAACheck(this);
        targetCombat.checkTakeDamageAA.Add(new CheckIfFrosted(targetCombat));
        myUI.combatPriority.text = string.Join(", ", combatPrio);

        qKeys.Add("Bonus Attack Speed");
        qKeys.Add("Total Damage Per Flurry");
        wKeys.Add("Physical Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new AttackSpeedBuff(4, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats), myStats.qSkill[0].basic.name));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        if (!targetStats.buffManager.buffs.TryAdd("Frosted", new FrostedBuff(2, targetStats.buffManager, myStats.wSkill[0].basic.name)))
        {
            targetStats.buffManager.buffs["Frosted"].duration = 2;
            targetStats.buffManager.buffs["Frosted"].source = myStats.wSkill[0].basic.name;
        }
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(1, targetStats.buffManager, myStats.rSkill[0].basic.name));
        if (!targetStats.buffManager.buffs.TryAdd("Frosted", new FrostedBuff(2, targetStats.buffManager, myStats.rSkill[0].basic.name)))
        {
            targetStats.buffManager.buffs["Frosted"].duration = 2;
            targetStats.buffManager.buffs["Frosted"].source = myStats.rSkill[0].basic.name;
        }
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }
}