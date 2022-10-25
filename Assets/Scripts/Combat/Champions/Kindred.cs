using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Kindred : ChampionCombat
{
    private CheckMoundingDread moundingDreadCheck;
    private bool wolfFrenzy;
    private int wolfFrenzyStack;
    public int currentMark = 0;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

        moundingDreadCheck = new CheckMoundingDread(this);
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
        checksQ.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        checkTakeDamage.Add(new CheckKindredR(this));
        checkTakeDamage.Add(new CheckKindredR(this));

        qKeys.Add("Physical Damage");
        qKeys.Add("Static Cooldown");
        wKeys.Add("Magic Damage");
        eKeys.Add("Additional Physical Damage");
        rKeys.Add("Heal");

        currentMark = 0;                      // we can change this later

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        WolfFrenzy();
        myStats.buffManager.buffs.Add("AttackSpeedBuff", new AttackSpeedBuff(4, myStats.buffManager, myStats.qSkill[0].basic.name, 0.25f + (0.5f * currentMark), "AttackSpeedBuff"));
        myStats.qCD = wolfFrenzy ? myStats.qSkill[0].UseSkill(4, qKeys[1], myStats, targetStats) : myStats.qSkill[0].basic.coolDown[4];   //shorter if block
        attackCooldown = 0;
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        StartCoroutine(WolfsFrenzy());
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("MoundingDread", new MoundingDreadBuff(4, targetStats.buffManager));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;
        myStats.buffManager.buffs.TryAdd("Untargetable", new UntargetableBuff(4, myStats.buffManager, myStats.rSkill[0].basic.name));
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.currentHealth += myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
        targetStats.currentHealth += myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        float missingHealth = myStats.maxHealth - myStats.currentHealth;
        if (wolfFrenzyStack >= 100 && missingHealth != 0)
        {
            UpdateTotalHeal(ref wSum, (45 + (2 * myStats.level)) * (1 + (missingHealth / myStats.maxHealth) > 0.8f ? 1f : (missingHealth / myStats.maxHealth) * 1.25f), "Wolf Frenzy Passive"); // Logic not sure here about how much health to add
            wolfFrenzyStack = 0;
        }
        else
        {
            WolfFrenzy();
        }
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        CheckMoundingDreadDamage();
    }

    public void WolfFrenzy()
    {
        wolfFrenzyStack += UnityEngine.Random.Range(2, 3);
    }

    public IEnumerator WolfsFrenzy()
    {
        wolfFrenzy = true;
        yield return new WaitForSeconds(8.5f);
        wolfFrenzy = false;
    }

    private void CheckMoundingDreadDamage()
    {
        if (moundingDreadCheck.Control())
        {
            //Apply's Crit as well so not sure about damage calculated
            UpdateTotalDamage(ref eSum, 2, new Damage(myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats), SkillDamageType.Phyiscal), myStats.eSkill[0].basic.name);
            targetStats.buffManager.buffs["MoundingDread"].Kill();
        }
        else if (myStats.buffManager.buffs.TryGetValue("MoundingDread", out Buff moundingDread))
        {
            moundingDread.value++;
            simulationManager.ShowText($"{myStats.name} Gained A Stack of Mounding Dread");
        }
    }
}