using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Akali : ChampionCombat
{
    public static float[] AkaliPassiveDamageByLevel = { 35, 38, 41, 44, 47, 50, 53, 62, 71, 8, 89, 98, 107, 122, 137, 152, 167, 182 };

    public bool eCast = false;
    private float timeSinceE = 0f;
    public bool rCast = false;
    public bool hRCast = false;
    public float timeSinceR = 0f;
    public float hTimeSinceR = 0f;
    public float remainingShroudTime;
    public float bannedFromShroud;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "E", "Q", "A" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        //autoattackcheck = new AkaliAACheck(this);
        checksE.Add(new CheckIfImmobilize(this));
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
        checksR.Add(new CheckIfImmobilize(this));
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Magic Damage");
        wKeys.Add("Shroud Duration");
        eKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Minimum Magic Damage");


        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceE += Time.fixedDeltaTime;
        timeSinceR += Time.fixedDeltaTime;
        hTimeSinceR += Time.fixedDeltaTime;
        remainingShroudTime -= remainingShroudTime > 0 ? Time.fixedDeltaTime : 0;
        bannedFromShroud -= bannedFromShroud > 0 ? Time.fixedDeltaTime : 0;
        if (bannedFromShroud <= 0 && remainingShroudTime > 0 && !MyBuffManager.buffs.ContainsKey("Untargetable"))
        {
            MyBuffManager.Add("Untargetable", new UntargetableBuff(remainingShroudTime, MyBuffManager, WSkill().basic.name));
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        bannedFromShroud = 1f;                                 //changes with game time
        if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff value)) value.Kill();
        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        //AssassinsMark();
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        remainingShroudTime = WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats);
        MyBuffManager.Add("Untargetable", new UntargetableBuff(remainingShroudTime, MyBuffManager, WSkill().basic.name));
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        if (!eCast)
        {
            bannedFromShroud = 1f;
            if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff value)) value.Kill();
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            if (UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile) != float.MinValue)
            {
                StartCoroutine(ShurikenFlip());
                myStats.eCD = 0.4f;
                timeSinceE = 0;
                //AssassinsMark();
            }
            else
            {
                myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
            }
        }
        else
        {
            bannedFromShroud = 1f;
            if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff value)) value.Kill();
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable);
            StopCoroutine(ShurikenFlip());
            eCast = false;
            myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel] - timeSinceE;
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        if (!rCast)
        {
            bannedFromShroud = 1f;
            if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff value)) value.Kill();
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
            StartCoroutine(PerfectExecution());
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
            myStats.rCD = 2.5f;
            timeSinceR = 0;
            //AssassinsMark();
        }
        else
        {
            bannedFromShroud = 1f;
            if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff value)) value.Kill();
            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
            float multiplier = (targetStats.maxHealth - targetStats.currentHealth) / targetStats.maxHealth > 0.7f ? 0.7f : targetStats.PercentMissingHealth * 0.0286f;
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(myStats.rSkill[0].UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats) * (1 + multiplier), SkillDamageType.Spell, SkillComponentTypes.Spellblockable), myStats.rSkill[0].basic.name);
            StopCoroutine(PerfectExecution());
            rCast = false;
            myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel] - timeSinceR;
        }
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        if (targetStats.buffManager.HasImmobilize) yield break;
        if (!hRCast)
        {
            yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
            StartCoroutine(HPerfectExecution(skillLevel));
            UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
            targetStats.rCD = 2.5f;
            hTimeSinceR = 0;
        }
        else
        {
            yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
            float multiplier = (myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth > 0.7f ? 0.7f : myStats.PercentMissingHealth * 0.0286f;
            UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, new Damage(myStats.rSkill[0].SylasUseSkill(skillLevel, rKeys[1], targetStats, myStats) * (1 + multiplier), SkillDamageType.Spell, SkillComponentTypes.Spellblockable), myStats.rSkill[0].basic.name);
            StopCoroutine(HPerfectExecution(skillLevel));
            hRCast = false;
            targetStats.rCD = (targetStats.rSkill[0].basic.coolDown[skillLevel] - hTimeSinceR) * 2;
        }
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        bannedFromShroud = 1f;
        if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff value)) value.Kill();
        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal, SkillComponentTypes.OnHit | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable));
    }

    /*public void AssassinsMark()
    {
        if (!targetStats.buffManager.buffs.TryAdd("AkaliPassiveBuff", new AkaliPassiveBuff(4, targetStats.buffManager, myStats.passiveSkill.skillName)))
        {
            targetStats.buffManager.buffs["AkaliPassiveBuff"].duration = 4;
            myStats.buffManager.buffs.Remove("AkaliPassive");
        }
    }*/

    public IEnumerator ShurikenFlip()
    {
        eCast = true;
        yield return new WaitForSeconds(3f);
        eCast = false;
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel] - timeSinceE;
    }

    public IEnumerator PerfectExecution()
    {
        rCast = true;
        yield return new WaitForSeconds(10f);
        rCast = false;
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel] - timeSinceR;
    }

    public IEnumerator HPerfectExecution(int skillLevel)
    {
        hRCast = true;
        yield return new WaitForSeconds(10f);
        hRCast = false;
        targetStats.rCD = (myStats.rSkill[0].basic.coolDown[skillLevel] - hTimeSinceR) * 2;
    }
}