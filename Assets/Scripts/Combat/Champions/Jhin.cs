using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Jhin : ChampionCombat
{
    public static float GetJhinInnateBonusADPercent(int level)
    {
        //TODO: account for crit and attack speed from items when implemented
        return level switch
        {
            < 9 => 0.03f + 0.01f * level,
            < 11 => 0.12f + 0.02f * (level - 9),
            _ => 0.16f + 0.04f * (level - 11)
        };
    }

    private int lotusTrapCharge = 2;
    private int whisperShot = 0;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "A", "W", "Q", "R" };

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

        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));

        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Minimum Physical Damage");
        wKeys.Add("Physical Damage");
        wKeys.Add("Root Duration");
        eKeys.Add("Magic Damage");
        rKeys.Add("Minimum Damage");

        base.UpdatePriorityAndChecks();
    }

    protected override void Start()
    {
        base.Start();
        myStats.AD += myStats.baseAD * GetJhinInnateBonusADPercent(myStats.level);
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        AddLotusTrapCharge();
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (whisperShot == 4)
        {
            whisperShot = 0;

            float damage = (myStats.AD * 1.75f) + ((targetCombat.myStats.maxHealth - targetCombat.myStats.currentHealth) * 0.25f); //crit + missing health max level
            if (damage < 0)
                damage = 0;

            if (autoattackcheck != null) damage = autoattackcheck.Control(new Damage(damage, SkillDamageType.Phyiscal)).value;

            aSum += targetCombat.TakeDamage(new Damage(damage, SkillDamageType.Phyiscal), $"{myStats.name}'s Auto Attack");
            hSum += HealHealth(damage * myStats.lifesteal, "Lifesteal");
            myUI.aaSum.text = aSum.ToString();
            myUI.healSum.text = hSum.ToString();

            attackCooldown = 1f / myStats.attackSpeed;
        }
        else
        {
            whisperShot++;
            AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        ApplyDeadlyFlourishMark(myStats.qSkill[0].basic.name);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        yield return StartCoroutine(base.ExecuteW());

        if (targetStats.buffManager.buffs.TryGetValue("Deadly Flourish Mark", out Buff deadlyFlourishMark))
            targetStats.buffManager.buffs.Add("Root", new RootBuff(myStats.wSkill[0].UseSkill(4, wKeys[1], myStats, targetStats), targetStats.buffManager, myStats.wSkill[0].basic.name));
    }

    public override IEnumerator ExecuteE()
    {
        if (lotusTrapCharge > 0)
        {
            if (!CheckForAbilityControl(checksE)) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            targetStats.buffManager.buffs.Add("Lotus Trap", new LotusTrapBuff(1, targetStats.buffManager, myStats.eSkill[0].basic.name));
            ApplyDeadlyFlourishMark(myStats.qSkill[0].basic.name);
            myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(10, myStats.buffManager, myStats.rSkill[0].basic.name, "CurtainCall"));
        yield return StartCoroutine(CurtainCall());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(10, targetStats.buffManager, myStats.rSkill[0].basic.name, "CurtainCall"));
        yield return StartCoroutine(HCurtainCall(skillLevel));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
    }

    private void ApplyDeadlyFlourishMark(string skillName)
    {
        if (myStats.wCD > 0) return;

        if (targetStats.buffManager.buffs.TryGetValue("Deadly Flourish Mark", out Buff deadlyFlourishMark))
            deadlyFlourishMark.duration = 4;
        else
            targetStats.buffManager.buffs.Add("Deadly Flourish Mark", new DeadlyFlourishBuff(4, targetStats.buffManager, skillName));
    }

    private void AddLotusTrapCharge()
    {
        if (lotusTrapCharge < 2)
            lotusTrapCharge++;
    }

    private IEnumerator CurtainCall()
    {
        int shots = 4;
        while (shots > 0)
        {
            if (shots == 1)
                UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]); //TODO: change to crit when Recep pushes new stuff
            else
                UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);

            shots--;
            yield return new WaitForSeconds(0.25f);
        }

        myStats.buffManager.buffs.Remove("Channeling");
    }

    private IEnumerator HCurtainCall(int skillLevel)
    {
        int shots = 4;
        while (shots > 0)
        {
            if (shots == 1)
                UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]); //TODO: change to crit when Recep pushes new stuff
            else
                UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);

            shots--;
            yield return new WaitForSeconds(0.25f);
        }

        myStats.buffManager.buffs.Remove("Channeling");
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }
}