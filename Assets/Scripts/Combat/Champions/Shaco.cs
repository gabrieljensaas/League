using Simulator.Combat;
using System.Collections;

public class Shaco : ChampionCombat
{
    public static float JackInTheBoxHP(int level)
    {
        return level switch
        {
            < 4 => 150,
            < 10 => 150 + ((level - 3) * 10),
            < 12 => 210 + ((level - 9) * 20),
            < 14 => 250 + ((level - 11) * 25),
            _ => 300 + ((level - 13) * 50)
        };
    }

    public static float JackInTheBoxHallucinationHP(int level)
    {
        return level switch
        {
            < 10 => 340 + (level - 5) * 20,
            < 12 => 420 + (level - 9) * 40,
            < 14 => 500 + (level - 11) * 50,
            _ => 600 + (level - 13) * 100
        };
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "W", "R", "A", "E" };

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

        qKeys.Add("Invisibility Duration");
        qKeys.Add("Bonus Physical Damage");
        wKeys.Add("Champion Disable Duration");
        wKeys.Add("Magic Damage");
        wKeys.Add("Increased Damage");
        eKeys.Add("Slow");
        eKeys.Add("Magic Damage");
        eKeys.Add("Increased Damage");
        eKeys.Add("Slow");
        rKeys.Add("Magic damage");
        rKeys.Add("Modified Magic Damage");
        rKeys.Add("Increased Modified Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        yield return base.ExecuteA();
        if (myStats.buffManager.buffs.TryGetValue("Untargetable", out Buff buff))
        {
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[1]);
            buff.Kill();
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats), myStats.buffManager, myStats.qSkill[0].basic.name));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        pets.Add(new JackInTheBox(this, JackInTheBoxHP(myStats.level), myStats.wSkill[0].UseSkill(4, wKeys[2], myStats, targetStats), 2, 100, 50, 5, 2));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));

        if (myStats.buffManager.buffs.TryGetValue("Untargetable", out Buff buff))
            buff.Kill();

        if (targetStats.PercentCurrentHealth > 0.3f)
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[1]);
        else
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[2]);

        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        pets.Add(new Hallucination(this, myStats.currentHealth, myStats.AD, myStats.attackSpeed, myStats.spellBlock, myStats.armor, 18));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }
}
