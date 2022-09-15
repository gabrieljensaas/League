using System.Collections;
using Simulator.Combat;
using UnityEngine;

public class Vex : ChampionCombat
{
    public float gloomTimer = 0;

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        gloomTimer -= Time.deltaTime;
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "Q", "W", "R", "A" };

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

        qKeys.Add("Magic Damage");
        wKeys.Add("Shield Strength");
        wKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        yield return base.ExecuteQ();
        Gloom(myStats.qSkill[0].name);
    }

    public override IEnumerator ExecuteW()
    {
        yield return base.ExecuteW();
        myStats.buffManager.shields.Add(myStats.wSkill[0].basic.name, new ShieldBuff(2.5f, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, eKeys[0], myStats, targetStats), myStats.wSkill[0].basic.name));
        Gloom(myStats.wSkill[0].name);
    }

    public override IEnumerator ExecuteE()
    {
        yield return base.ExecuteE();
        Gloom(myStats.eSkill[0].name);
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        Gloom(myStats.rSkill[0].name);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        Gloom(myStats.rSkill[0].name);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[4];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[1]);
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[4] * 2;
    }

    private void Doom()
    {
        //CHECK FOR DASHES WITH RECEP
    }

    private void Gloom(string skillName)
    {
        if(gloomTimer >= 0)
        {
            targetStats.buffManager.buffs.Add("Flee", new FleeBuff(GetVexDoomFear(myStats.level), targetStats.buffManager, skillName));
            gloomTimer = GetVexDoomCooldown(myStats.level);
        }
    }

    public static float GetVexDoomCooldown(int level)
    {
        return level switch
        {
            < 6 => 25,
            < 11 => 22,
            < 16 => 19,
            _ => 16
        };
    }

    public static float GetVexDoomFear(int level)
    {
        return level switch
        {
            < 6 => 0.75f,
            < 9 => 1f,
            < 13 => 1.25f,
            _ => 1.5f
        };
    }
}
