using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class LeeSin : ChampionCombat
{
    public bool qCast = false;
    public float timeSinceQ = 0f;
    public bool wCast = false;
    private float timeSinceW = 0f;
    public bool eCast = false;
    private float timeSinceE = 0f;
    private int flurryStack = 0;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "W", "E", "A" };

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

        qKeys.Add("Physical Damage");
        wKeys.Add("Shield Strength");
        wKeys.Add("Bonus Drain");
        eKeys.Add("Magic Damage");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceQ += Time.deltaTime;
        timeSinceW += Time.deltaTime;
        timeSinceE += Time.deltaTime;
    }


    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        if (!qCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref qSum, 2, myStats.qSkill[0], 4, qKeys[0]);
            StartCoroutine(SonicWave());
            myStats.qCD = 0.4f;
            timeSinceQ = 0;
            Flurry();
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
            if (myStats.buffManager.HasImmobilize) yield break;
            float extraDamage = myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats) * (1 + ((targetStats.maxHealth - targetStats.currentHealth) / targetStats.maxHealth));
            UpdateAbilityTotalDamage(ref qSum, 4, new Damage(extraDamage, SkillDamageType.Spell), myStats.qSkill[0].basic.name);
            StopCoroutine(SonicWave());
            Flurry();
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4] - timeSinceQ;

        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        if (!wCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
            if (myStats.buffManager.HasImmobilize) yield break;
            myStats.buffManager.shields.Add("ShieldBuff", new ShieldBuff(2f, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), "ShieldBuff"));
            StartCoroutine(SafeGuard());
            myStats.wCD = 0.4f;
            timeSinceW = 0;
            Flurry();
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
            myStats.buffManager.buffs.Add("LifeStealBuff", new LifeStealBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[1], myStats, targetStats), "LifeStealBuff"));
            // Need to add SpellVamp Buff 
            StopCoroutine(SafeGuard());
            Flurry();
            myStats.wCD = myStats.wSkill[0].basic.coolDown[4] - timeSinceW;
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        if (!eCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
            StartCoroutine(Tempest());
            myStats.eCD = 0.4f;
            timeSinceE = 0;
            Flurry();
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            //only Slows them.
            StopCoroutine(Tempest());
            Flurry();
            myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.buffManager.buffs.Add("AirBorneBuff", new AirborneBuff(1f, targetStats.buffManager, "AirBorneBuff"));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if (flurryStack == 1) myStats.buffManager.buffs["Flurry"].Kill();
        else if (flurryStack > 0)
        {
            flurryStack--;
            myStats.buffManager.buffs["Flurry"].value = flurryStack;
            myStats.buffManager.buffs["Flurry"].duration = 3;
        }
    }

    public IEnumerator SonicWave()
    {
        qCast = true;
        yield return new WaitForSeconds(3f);
        qCast = false;
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4] - timeSinceQ;
    }

    public IEnumerator SafeGuard()
    {
        wCast = true;
        yield return new WaitForSeconds(3f);
        wCast = false;
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4] - timeSinceW;
    }
    public IEnumerator Tempest()
    {
        eCast = true;
        yield return new WaitForSeconds(3f);
        eCast = false;
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
    }

    public void Flurry()
    {
        if (!myStats.buffManager.buffs.TryAdd("Flurry", new AttackSpeedBuff(3, myStats.buffManager, myStats.passiveSkill.skillName, 0.4f * myStats.baseAttackSpeed, "Flurry")))
        {
            myStats.buffManager.buffs["Flurry"].value = 2;
            myStats.buffManager.buffs["Flurry"].duration = 3;
        }
    }
}