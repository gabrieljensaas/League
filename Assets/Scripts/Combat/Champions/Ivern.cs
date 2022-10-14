using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Ivern : ChampionCombat
{
    private int brushMakerCharge = 3;
    public bool inBrush;
    private float timeSinceBush = 0;

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

        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));
        checksQ.Add(new CheckIfImmobilize(this));

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Root Duration");

        wKeys.Add("Bonus Magic Damage");

        eKeys.Add("Shield Strength");
        eKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        timeSinceBush += Time.deltaTime;
        AddBrushmakerCharge();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.Spell), QSkill().basic.name);
        TargetBuffManager.Add("RootBuff", new RootBuff(QSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), TargetBuffManager, "RootCaller"));
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }
    public override IEnumerator ExecuteW()
    {
        if (brushMakerCharge > 0)
        {
            if (!CheckForAbilityControl(checksW)) yield break;
            yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));

            if (timeSinceBush >= 30)
            {
                MyBuffManager.Add("Brushmaker", new BrushMakerBuff(30f, MyBuffManager, WSkill().basic.name));
                timeSinceBush = 0;
            }
            myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        MyBuffManager.Add("ShieldBuff", new ShieldBuff(2f, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), "Triggerseed"));
        yield return new WaitForSeconds(2f);
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[1]);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        pets.Add(new Daisy(this, 3900, 70 + (myStats.AP * 0.3f), 0.7f, 100, 100));
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        if (inBrush)
        {
            AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), SkillDamageType.Spell));
        }
    }

    private void AddBrushmakerCharge()
    {
        if (brushMakerCharge < 3)
            brushMakerCharge++;
    }

}
public class BrushMakerBuff : Buff
{
    private Ivern ivern;
    public BrushMakerBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has been in brush for {duration} seconds because of {source}!");
        ivern.inBrush = true;
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        ivern.inBrush = false;
        manager.simulationManager.ShowText($"{manager.stats.name} is not in brush anymore!");
        manager.buffs.Remove("Brushmaker");
    }
}
