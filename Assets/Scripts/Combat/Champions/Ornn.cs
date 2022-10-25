using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Ornn : ChampionCombat
{
    private bool rRecast = false;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "Q", "E", "A" };

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
        checksQ.Add(new CheckIfImmuneToCC(this, myStats.wSkill[0].basic.name));
        checksW.Add(new CheckIfImmuneToCC(this, myStats.wSkill[0].basic.name));
        checksE.Add(new CheckIfImmuneToCC(this, myStats.wSkill[0].basic.name));
        checksR.Add(new CheckIfImmuneToCC(this, myStats.wSkill[0].basic.name));
        checksA.Add(new CheckIfImmuneToCC(this, myStats.wSkill[0].basic.name));
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));
        checksE.Add(new CheckIfImmobilize(this));
        checksQ.Add(new CheckIfUnableToAct(this));
        checksW.Add(new CheckIfUnableToAct(this));
        checksE.Add(new CheckIfUnableToAct(this));
        checksR.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));

        qKeys.Add("Physical Damage");
        wKeys.Add("Magic Damage Per Tick");
        eKeys.Add("Physical Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(1.125f);
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.1f, targetStats.buffManager, myStats.qSkill[0].basic.name));  //needs research for actual airborne duration
        if (targetStats.buffManager.buffs.TryGetValue("Brittle", out Buff value))
        {
            value.Kill();
            UpdateTotalDamage(ref pSum, 4, new Damage((10 + (8 / 17 * (myStats.level - 1))) * 0.01f * targetStats.maxHealth, SkillDamageType.Spell), "Living Forge");
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("UnableToAct", new UnableToActBuff(0.75f, myStats.buffManager, myStats.wSkill[0].basic.name));
        myStats.buffManager.buffs.Add(myStats.wSkill[0].basic.name, new ImmuneToCCBuff(0.75f, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].basic.name));
        StartCoroutine(BellowsBreath());
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(1.5f, targetStats.buffManager, myStats.qSkill[0].basic.name));  //needs research for actual airborne duration
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(1.5f, targetStats.buffManager, myStats.qSkill[0].basic.name));
        if (targetStats.buffManager.buffs.TryGetValue("Brittle", out Buff value))
        {
            value.Kill();
            UpdateTotalDamage(ref pSum, 4, new Damage((10 + (8 / 17 * (myStats.level - 1))) * 0.01f * targetStats.maxHealth, SkillDamageType.Spell), "Living Forge");
        }

        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!rRecast)
        {
            if (!CheckForAbilityControl(checksR)) yield break;

            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
            if (!targetStats.buffManager.buffs.TryAdd("Brittle", new BrittleBuff(3f, targetStats.buffManager, myStats.wSkill[0].basic.name))) targetStats.buffManager.buffs["Brittle"].duration = 3f;
            UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
            myStats.rCD = myStats.rSkill[0].basic.coolDown[2];

        }
        else
        {
            if (!CheckForAbilityControl(checksR)) yield break;
            if (myStats.buffManager.HasImmobilize) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(1f, targetStats.buffManager, myStats.qSkill[0].basic.name));  //needs research for actual airborne duration
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(1f, targetStats.buffManager, myStats.qSkill[0].basic.name));
            if (targetStats.buffManager.buffs.TryGetValue("Brittle", out Buff value))
            {
                value.Kill();
                UpdateTotalDamage(ref pSum, 4, new Damage((10 + (8 / 17 * (myStats.level - 1))) * 0.01f * targetStats.maxHealth, SkillDamageType.Spell), "Living Forge");
            }

            UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
            rRecast = false;
        }
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (targetStats.buffManager.buffs.TryGetValue("Brittle", out Buff value))
        {
            value.Kill();
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.1f, targetStats.buffManager, "Ornn's Auto Attack"));
            UpdateTotalDamage(ref pSum, 4, new Damage((10 + (8 / 17 * (myStats.level - 1))) * 0.01f * targetStats.maxHealth, SkillDamageType.Spell), "Living Forge");
        }
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
    }

    public IEnumerator BellowsBreath()
    {
        yield return new WaitForSeconds(0.15f);
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        yield return new WaitForSeconds(0.15f);
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        yield return new WaitForSeconds(0.15f);
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        yield return new WaitForSeconds(0.15f);
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        yield return new WaitForSeconds(0.15f);
        UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        if (!targetStats.buffManager.buffs.TryAdd("Brittle", new BrittleBuff(3f, targetStats.buffManager, myStats.wSkill[0].basic.name))) targetStats.buffManager.buffs["Brittle"].duration = 3f;
    }

    public IEnumerator CallOfTheForgeGod()
    {
        yield return new WaitForSeconds(1.25f);
        rRecast = true;
        yield return new WaitForSeconds(0.75f);
        rRecast = false;
    }
}