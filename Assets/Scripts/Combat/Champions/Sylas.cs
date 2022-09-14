using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Sylas : ChampionCombat
{
    public int UnshackledStack = 0;
    public bool eCast = false;
    private float timeSinceE = 0f;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

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
        checksW.Add(new CheckIfImmobilize(this));
        checksE.Add(new CheckIfImmobilize(this));
        autoattackcheck = new SylasAACheck(this,this);

        qKeys.Add("Magic Damage");
        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage");
        wKeys.Add("Minimum Heal");
        eKeys.Add("Magic Damage");
        rKeys.Add("");

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();

        timeSinceE += Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        Passive();
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(0.6F);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[1]);
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        Passive();
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        UpdateTotalHeal(ref hSum, (1 + Mathf.Clamp01(((myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth) % 0.006f)) * myStats.wSkill[0].UseSkill(4, wKeys[1], myStats, targetStats), myStats.wSkill[0].basic.name);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        if (!eCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            StartCoroutine(Abscond());
            myStats.eCD = 0.2f;
            timeSinceE = 0;
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[1].basic.castTime));
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.5f, targetStats.buffManager, myStats.eSkill[1].basic.name));
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.5f, targetStats.buffManager, myStats.eSkill[1].basic.name));
            eCast = false;
            StopCoroutine(Abscond());
            myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
        }
    }

    public virtual IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
    }

    public void Passive()
    {
        if (UnshackledStack != 3) UnshackledStack++;
        StopCoroutine(Unshackled());
        StartCoroutine(Unshackled());
    }

    public IEnumerator Unshackled()
    {
        myStats.buffManager.buffs.TryAdd("Unshackled", new AttackSpeedBuff(float.MaxValue, myStats.buffManager, "Petricite Burst", myStats.baseAttackSpeed * 1.25f, "Unshackled"));
        yield return new WaitForSeconds(4);
        UnshackledStack = 0;
    }

    public IEnumerator Abscond()
    {
        eCast = true;
        yield return new WaitForSeconds(3.5f);
        eCast = false;
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4] - timeSinceE;
    }
}