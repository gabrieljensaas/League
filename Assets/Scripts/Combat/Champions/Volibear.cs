using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Volibear : ChampionCombat
{
    private static float LightningClaws(int value)
    {
        return value switch
        {
            <= 3 => 1,
            <= 6 => 2,
            <= 13 => 3,
            _ => 4
        };
    }

    private int pStack;
    private float timeSinceWounded;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "E", "W", "A" };

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
        checksR.Add(new CheckIfImmobilize(this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));
        autoattackcheck = new VolibearAACheck(this);

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Bonus Physical Damage");

        wKeys.Add("Physical Damage");
        wKeys.Add("Increased Damage");
        wKeys.Add("Heal");

        eKeys.Add("Magic Damage");

        rKeys.Add("Bonus Health");
        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceWounded += Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        MyBuffManager.Add("ThunderingSmashBuff", new ThunderingSmashBuff(4, MyBuffManager, QSkill().basic.name));
        TargetBuffManager.Add("StunBuff", new StunBuff(1f, TargetBuffManager, QSkill().basic.name));
        myStats.qCD = QSkill().basic.coolDown[4];
        attackCooldown = 0;
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW) || myStats.wLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        if (timeSinceWounded > 8)
        {
            UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
        }
        else
        {
            UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable);
            UpdateTotalHeal(ref wSum, WSkill().UseSkill(myStats.wLevel, wKeys[2], myStats, targetStats), WSkill().basic.name);
        }
        timeSinceWounded = 0;
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        yield return new WaitForSeconds(2f);
        MyBuffManager.Add("ShieldBuff", new ShieldBuff(3f, MyBuffManager, ESkill().basic.name, 0.14f * myStats.maxHealth, "SkySplitter"));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        MyBuffManager.Add("ImmunetoCC", new ImmuneToCCBuff(0.2f, MyBuffManager, RSkill().basic.name, "Stormbringer"));
        MyBuffManager.Add("StormbringerBuff", new StormbringerBuff(12, MyBuffManager, RSkill().basic.name, (int)RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats)));
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[1]);
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        StopCoroutine(PStackExpired());
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if (pStack >= 5) pStack = 5;
        else pStack++;
        if (pStack == 5)
        {
            AutoAttack(new Damage(10 + LightningClaws(myStats.level), SkillDamageType.Spell));
        }
        myStats.buffManager.buffs.Remove("RelentlessStorm");
        myStats.buffManager.buffs.Add("RelentlessStorm", new AttackSpeedBuff(6f, myStats.buffManager, "RelentlessStorm", 0.05f * pStack, "RelentlessStorm"));
        StartCoroutine(PStackExpired());
    }

    private IEnumerator PStackExpired()
    {
        yield return new WaitForSeconds(6);
        pStack = 0;
        myStats.buffManager.buffs.Remove("RelentlessStorm");
    }
}

public class ThunderingSmashBuff : Buff
{
    public ThunderingSmashBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;

        manager.simulationManager.ShowText($"{manager.stats.name} gained Smash from {source} for {duration} Seconds!");
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has ThunderingSmash from {source}!");
        manager.buffs.Remove("ThunderingSmash");
    }
}

public class VolibearAACheck : Check
{
    public VolibearAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.targetStats.buffManager.buffs.TryGetValue("ThunderingSmash", out Buff buff))
        {
            damage.value = combat.QSkill().UseSkill(combat.myStats.qLevel, combat.qKeys[0], combat.myStats, combat.targetStats);
            buff.Kill();
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}

public class StormbringerBuff : Buff
{
    public StormbringerBuff(float duration, BuffManager manager, string source, int HP) : base(manager)
    {
        manager.stats.maxHealth += HP;
        manager.stats.currentHealth += HP;
        manager.stats.bonusHP += HP;
        value = HP;
        base.source = source;
        base.duration = duration;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {HP} Health from {source}! ");
    }

    public override void Update()
    {
        if (!paused) duration -= Time.fixedDeltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {value} Extra Health From {source}!");
        manager.stats.maxHealth -= value;
        manager.stats.currentHealth -= value;
        manager.stats.bonusHP -= value;
        manager.buffs.Remove("Stormbringer");
    }
}