using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Aatrox : ChampionCombat
{
    public static float[] AatroxPassiveCooldownByLevelTable = { 24, 23.29f, 22.59f, 21.88f, 21.18f, 20.47f, 19.76f, 19.06f, 18.35f, 17.65f, 16.94f, 16.24f, 15.53f, 14.82f, 14.12f, 13.41f, 12.71f, 12f };
    public float pCD = 0;
    private int qCounter = 0;
    private float timeSinceLastQ = 0f;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "W", "A", "E" };

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
        checksE.Add(new CheckIfImmobilize(this));
        targetCombat.checkTakeDamageAAPostMitigation.Add(new CheckIfEnemyHasDeathBringerStance(targetCombat, this));
        targetCombat.checkTakeDamageAAPostMitigation.Add(new CheckForAatroxEHeal(targetCombat, this));
        targetCombat.checkTakeDamageAbilityPostMitigation.Add(new CheckForAatroxEHeal(targetCombat, this));

        qKeys.Add("First Sweetspot Damage");
        qKeys.Add("Second Sweetspot Damage");
        qKeys.Add("Third Sweetspot Damage");
        wKeys.Add("Physical Damage");
        eKeys.Add("Healing");
        eKeys.Add("World Ender Increased Healing");
        rKeys.Add("Bonus Attack Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        timeSinceLastQ += Time.fixedDeltaTime;
        if (timeSinceLastQ > 4) qCounter = 0;
        pCD -= Time.fixedDeltaTime;
        if (pCD <= 0 && !myStats.buffManager.buffs.ContainsKey("DeathbringerStance")) myStats.buffManager.buffs.Add("DeathbringerStance", new DeathbringerStanceBuff(float.MaxValue, myStats.buffManager, "Deathbringer Stance"));
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == 0) yield break;
        if (qCounter == 0 && myStats.qCD > 0) yield break;
        if (timeSinceLastQ < 1f) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if (qCounter == 0)
        {
            myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
            timeSinceLastQ = 0f;
            qCounter++;
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.25f, targetStats.buffManager, QSkill().basic.name));
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], buffNames: new string[] { "Airborne" }, skillComponentTypes: SkillComponentTypes.Spellblockable);
        }
        else if (qCounter == 2)
        {
            timeSinceLastQ = 0f;
            qCounter = 0;
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.25f, targetStats.buffManager, QSkill().basic.name));
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[2], buffNames: new string[] { "Airborne" }, skillComponentTypes: SkillComponentTypes.Spellblockable);
        }
        else
        {
            timeSinceLastQ = 0f;
            qCounter++;
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.25f, targetStats.buffManager, QSkill().basic.name));
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[1], buffNames: new string[] { "Airborne" }, skillComponentTypes: SkillComponentTypes.Spellblockable);
        }
        pCD -= 4;
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == 0) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        if (UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable) > 0)
        {
            yield return new WaitForSeconds(1.5f);
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.1f, targetStats.buffManager, myStats.qSkill[0].basic.name));  //pulled airborne needs research
            UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0], skillComponentTypes: SkillComponentTypes.OnHit, buffNames: new string[] { "Airborne" });
            pCD -= 2;
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == 0) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, SkillComponentTypes.Dash), ESkill().basic.name);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        attackCooldown = 0;
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == 0) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new AttackDamageBuff(10, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), myStats.rSkill[0].basic.name));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new AttackDamageBuff(10, targetStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(skillLevel, rKeys[0], targetStats, myStats), myStats.rSkill[0].basic.name));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal, SkillComponentTypes.OnHit | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable)).damage > 0)
        {
            if (MyBuffManager.buffs.ContainsKey("DeathbringerStance"))
            {
                UpdateAbilityTotalDamage(ref pSum, 5, new Damage(targetStats.maxHealth * (5 + (7 / 17 * (myStats.level - 1))), SkillDamageType.Phyiscal, SkillComponentTypes.ProcDamage | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable), "Deathbringer Stance");
            }
            else pCD -= 2;
        }
    }
}