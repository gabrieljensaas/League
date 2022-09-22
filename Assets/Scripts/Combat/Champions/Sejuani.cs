using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Sejuani : ChampionCombat
{
    private float timeSinceEPassive = 0f;
    private int frost = 0;
    public bool HasFrostArmor = true;
    public float passiveTimer = 0f;

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
        checksQ.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));
        checkTakeDamageAAPostMitigation.Add(new CheckFrostArmor(this, this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckFrostArmor(this, this));

        qKeys.Add("Magic Damage");
        wKeys.Add("Physical Damage");
        wKeys.Add("Physical Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");

        myStats.armor += ((myStats.armor - myStats.baseArmor) * 0.5f) + 10;
        myStats.spellBlock += ((myStats.spellBlock - myStats.baseSpellBlock) * 0.5f) + 10;

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceEPassive += Time.deltaTime;
        passiveTimer += Time.deltaTime;

        if (passiveTimer >= 12 - (6 / 17 * (myStats.level - 1)))
        {
            myStats.armor += ((myStats.armor - myStats.baseArmor) * 0.5f) + 10;
            myStats.spellBlock += ((myStats.spellBlock - myStats.baseSpellBlock) * 0.5f) + 10;
            HasFrostArmor = true;
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        passiveTimer = 0;
        CheckIfFrozen();
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.5f, targetStats.buffManager, myStats.qSkill[0].basic.name));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.25f));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        passiveTimer = 0;
        CheckIfFrozen();
        if (timeSinceEPassive > 10 && !targetStats.buffManager.buffs.ContainsKey("Frozen")) frost++;
        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime - 0.25f));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[1]);
        passiveTimer = 0;
        CheckIfFrozen();
        if (timeSinceEPassive > 10 && !targetStats.buffManager.buffs.ContainsKey("Frozen")) frost++;
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;
        if (frost < 4) yield break;
        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        passiveTimer = 0;
        CheckIfFrozen();
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(1, targetStats.buffManager, myStats.eSkill[0].basic.name));
        targetStats.buffManager.buffs.Add("Frozen", new FrozenBuff(1f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        frost = 0;
        timeSinceEPassive = 0f;
        attackCooldown = 0f;
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        passiveTimer = 0;
        CheckIfFrozen();
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(1f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        if (timeSinceEPassive > 10) targetStats.buffManager.buffs.Add("Frozen", new FrozenBuff(1f, targetStats.buffManager, myStats.rSkill[0].basic.name));
        passiveTimer = 0;
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack();
        passiveTimer = 0;
        CheckIfFrozen();
        if (timeSinceEPassive >= 10 && !targetStats.buffManager.buffs.ContainsKey("Frozen")) frost++;
    }
    private void CheckIfFrozen()
    {
        if (targetStats.buffManager.buffs.TryGetValue("Frozen", out Buff frozen))
        {
            UpdateAbilityTotalDamage(ref pSum, 4, targetStats.maxHealth * 0.1f, "Frozen", SkillDamageType.Spell);
            passiveTimer = 0;
            frozen.Kill();
        }
    }

    public IEnumerator FrostArmor()
    {
        yield return new WaitForSeconds(3f);
        if (HasFrostArmor)
        {
            HasFrostArmor = false;
            myStats.armor -= ((myStats.armor - myStats.baseArmor) * 0.5f) + 10;
            myStats.spellBlock -= ((myStats.spellBlock - myStats.baseSpellBlock) * 0.5f) + 10;
        }
    }
}