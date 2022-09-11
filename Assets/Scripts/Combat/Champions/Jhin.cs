using System.Collections;
using Simulator.Combat;
using UnityEngine;

public class Jhin : ChampionCombat
{
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

        targetCombat.checksQ.Add(new CheckIfRooted(targetCombat));
        targetCombat.checksW.Add(new CheckIfRooted(targetCombat));
        targetCombat.checksE.Add(new CheckIfRooted(targetCombat));
        targetCombat.checksR.Add(new CheckIfRooted(targetCombat));
        targetCombat.checksA.Add(new CheckIfRooted(targetCombat));

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
        myStats.AD += myStats.baseAD * Constants.GetJhinInnateBonusADPercent(myStats.level);
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
        if(whisperShot == 4)
        {
            whisperShot = 0;

            float damage = (myStats.AD * 1.75f) + ((targetCombat.myStats.maxHealth - targetCombat.myStats.currentHealth) * 0.25f); //crit + missing health max level
            if (damage < 0)
                damage = 0;

            if (autoattackcheck != null) damage = autoattackcheck.Control(damage);

            aSum += targetCombat.TakeDamage(damage, $"{myStats.name}'s Auto Attack", SkillDamageType.Phyiscal, true);
            hSum += HealHealth(damage * myStats.lifesteal, "Lifesteal");
            myUI.aaSum.text = aSum.ToString();
            myUI.healSum.text = hSum.ToString();

            attackCooldown = 1f / myStats.attackSpeed;
        }
        else
        {
            whisperShot++;
            AutoAttack();
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
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
        if(lotusTrapCharge > 0)
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
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(10, myStats.buffManager, myStats.rSkill[0].basic.name, "Curtain Call"));

        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    private void ApplyDeadlyFlourishMark(string skillName)
    {
        if (myStats.wCD > 0) return;

        if(targetStats.buffManager.buffs.TryGetValue("Deadly Flourish Mark", out Buff deadlyFlourishMark))
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
        while(shots > 0)
        {
            if(shots == 1)
                UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 4, rKeys[0]); //TODO: change to crit when Recep pushes new stuff
            else
                UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 4, rKeys[0]);

            shots--;
            yield return new WaitForSeconds(0.25f);
        }

        myStats.buffManager.buffs.Remove("Channeling");
    }
}
