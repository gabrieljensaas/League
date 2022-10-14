using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Nidalee : ChampionCombat
{
    private bool isCougarForm = false;
    private bool cougarQReady = true;
    private bool cougarWReady = true;
    private bool cougarEReady = true;
    private bool cougarRReady = true;

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "W", "E", "R", "A" };

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

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Minimum Magic Damage");
        qKeys.Add("Maximum Magic Damage");
        qKeys.Add("Minimum Magic Damage");
        qKeys.Add("Maximum Magic Damage");
        qKeys.Add("Increased Damage Modifier");
        qKeys.Add("Maximum Increased Damage");
        qKeys.Add("Maximum Increased Damage");
        qKeys.Add("Prowl-Enhanced Minimum Damage");
        qKeys.Add("Prowl-Enhanced Maximum Damage");

        wKeys.Add("Magic Damage Per Tick");
        wKeys.Add("Total Magic Damage");
        wKeys.Add("Reduced Cooldown");
        wKeys.Add("Magic Damage");

        eKeys.Add("Bonus Attack Speed");
        eKeys.Add("Minimum Heal");
        eKeys.Add("Maximum Heal");
        eKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        if (isCougarForm)
        {
            if (!cougarQReady) yield break;
            yield return StartCoroutine(StartCastingAbility(QSkill(1).basic.castTime));

            if (MyBuffManager.buffs.TryGetValue("HuntMarkBuff", out Buff buff))
            {
                UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill(1).UseSkill(3, qKeys[2], myStats, targetStats) * (QSkill(1).UseSkill(3, qKeys[4], myStats, targetStats) + 1) * 1.4f, SkillDamageType.Spell), QSkill(1).name);
                buff.Kill();
            }
            else
                UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill(1).UseSkill(3, qKeys[2], myStats, targetStats) * (QSkill(1).UseSkill(3, qKeys[4], myStats, targetStats) + 1), SkillDamageType.Spell), QSkill(1).name);

            StartCoroutine(CougarQCD());
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[1]);
            myStats.qCD = QSkill().basic.coolDown[4];
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        if (isCougarForm)
        {
            if (!cougarWReady) yield break;
            yield return StartCoroutine(StartCastingAbility(WSkill(1).basic.castTime));
            UpdateAbilityTotalDamage(ref wSum, 1, WSkill(1), myStats.wLevel, wKeys[0]);

            if (MyBuffManager.buffs.ContainsKey("HuntMarkBuff"))
                StartCoroutine(CougarWCD(WSkill(1).UseSkill(3, wKeys[2], myStats, targetStats)));
            else
                StartCoroutine(CougarWCD(0));
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
            TargetBuffManager.Add("BushwackBuff", new BushwackBuff(5.5f, TargetBuffManager, WSkill().name, 1.5f));
            myStats.wCD = WSkill().basic.coolDown[4];
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        if (isCougarForm)
        {
            if (!cougarEReady) yield break;
            yield return StartCoroutine(StartCastingAbility(ESkill(1).basic.castTime));

            if (MyBuffManager.buffs.TryGetValue("HuntMarkBuff", out Buff buff))
            {
                UpdateAbilityTotalDamage(ref eSum, 2, new Damage(ESkill(1).UseSkill(3, eKeys[3], myStats, targetStats) * 1.4f, SkillDamageType.Spell), ESkill(1).name);
                buff.Kill();
            }
            else
                UpdateAbilityTotalDamage(ref eSum, 2, new Damage(ESkill(1).UseSkill(3, eKeys[3], myStats, targetStats), SkillDamageType.Spell), ESkill(1).name);

            StartCoroutine(CougarECD());
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
            UpdateTotalHeal(ref hSum, ESkill().UseSkill(4, eKeys[1], myStats, targetStats) * (1 + (myStats.PercentMissingHealth * 0.95f)), ESkill().name);
            MyBuffManager.Add("PrimalSurgeAS", new AttackSpeedBuff(7, MyBuffManager, ESkill().name, ESkill().UseSkill(4, eKeys[0], myStats, targetStats), "PrimalSurgeAS"));
            myStats.eCD = ESkill().basic.coolDown[4];
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;
        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));

        if (isCougarForm)
        {
            isCougarForm = false;
            StartCoroutine(CougarRCD());
        }
        else
        {
            isCougarForm = true;
            myStats.rCD = RSkill().basic.coolDown[2];
        }
    }

    public IEnumerator CougarQCD()
    {
        cougarQReady = false;
        yield return new WaitForSeconds(QSkill(1).basic.coolDown[4]);
        cougarQReady = true;
    }

    public IEnumerator CougarWCD(float reducedTime)
    {
        cougarWReady = false;
        yield return new WaitForSeconds(WSkill(1).basic.coolDown[4] - reducedTime);
        cougarWReady = true;
    }

    public IEnumerator CougarECD()
    {
        cougarEReady = false;
        yield return new WaitForSeconds(ESkill(1).basic.coolDown[4]);
        cougarEReady = true;
    }

    public IEnumerator CougarRCD()
    {
        cougarRReady = false;
        yield return new WaitForSeconds(RSkill(1).basic.coolDown[2]);
        cougarRReady = true;
    }

    private class HuntMarkBuff : Buff
    {
        public HuntMarkBuff(float duration, BuffManager manager, string source) : base(manager)
        {
            base.duration = duration;
            base.source = source;
            manager.simulationManager.ShowText($"{manager.stats.name} has been Marked for {duration}!");
        }

        public override void Update()
        {
            duration -= Time.deltaTime;
            if (duration <= 0) Kill();

        }
        public override void Kill()
        {
            manager.simulationManager.ShowText($"{manager.stats.name} is no longer Marked!");
            manager.buffs.Remove("HuntMarkBuff");
        }
    }

    public class BushwackBuff : Buff
    {
        private float activationTime;
        public bool isActive;
        private float tickTimer = 0;

        public BushwackBuff(float duration, BuffManager manager, string source, float activationTime) : base(manager)
        {
            base.duration = duration;
            base.source = source;
            this.activationTime = activationTime;
            manager.simulationManager.ShowText($"{manager.stats.name} has Bushwack for {duration} seconds from {source}!");
        }

        public override void Update()
        {
            duration -= Time.deltaTime;
            if (duration <= 0) Kill();

            if (!isActive && duration <= activationTime) isActive = true;

            tickTimer += Time.deltaTime;
            if (tickTimer >= 1 && isActive)
            {
                manager.combat.targetCombat.UpdateAbilityTotalDamage(ref manager.combat.wSum, 1, manager.combat.WSkill(), manager.combat.myStats.wLevel, manager.combat.wKeys[0]);
                tickTimer = 0;
            }
        }

        public override void Kill()
        {
            manager.simulationManager.ShowText($"{manager.stats.name} no longer has Bushwack from {source}!");
            manager.buffs.Remove("BushwackBuff");
        }
    }
}
