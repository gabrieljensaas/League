using Simulator.Combat;
using System.Collections;

public class Ashe : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "A", "W", "R", "" };

        checksQ.Add(new CheckIfCasting(this));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckAsheQ(this));
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));
        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        autoattackcheck = new AsheAACheck(this);
        targetCombat.checkTakeDamageAA.Add(new CheckIfFrosted(targetCombat));


        qKeys.Add("Bonus Attack Speed");
        qKeys.Add("Total Damage Per Flurry");
        wKeys.Add("Physical Damage");
        rKeys.Add("Magic Damage");

        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new AttackSpeedBuff(4, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), myStats.qSkill[0].basic.name));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        if (!targetStats.buffManager.buffs.TryAdd("Frosted", new FrostedBuff(2, targetStats.buffManager, myStats.wSkill[0].basic.name)))
        {
            targetStats.buffManager.buffs["Frosted"].duration = 2;
            targetStats.buffManager.buffs["Frosted"].source = myStats.wSkill[0].basic.name;
        }
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(1, targetStats.buffManager, myStats.rSkill[0].basic.name));
        if (!targetStats.buffManager.buffs.TryAdd("Frosted", new FrostedBuff(2, targetStats.buffManager, myStats.rSkill[0].basic.name)))
        {
            targetStats.buffManager.buffs["Frosted"].duration = 2;
            targetStats.buffManager.buffs["Frosted"].source = myStats.rSkill[0].basic.name;
        }
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
    }
    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Stun", new StunBuff(1, myStats.buffManager, myStats.rSkill[0].basic.name));
        if (!myStats.buffManager.buffs.TryAdd("Frosted", new FrostedBuff(2, myStats.buffManager, myStats.rSkill[0].basic.name)))
        {
            myStats.buffManager.buffs["Frosted"].duration = 2;
            myStats.buffManager.buffs["Frosted"].source = myStats.rSkill[0].basic.name;
        }
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
    }
}