using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Viego : ChampionCombat
{
    private bool wCast;
    private float timeSinceW;
    private float timeChanneled;
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
        checksR.Add(new CheckIfImmobilize(this));
        checksR.Add(new CheckIfExecutes(this, "R"));
        autoattackcheck = new ViegoAACheck(this);

        qKeys.Add("Bonus Physical Damage");
        qKeys.Add("Physical Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Bonus Attack Speed");
        rKeys.Add("Bonus Physical Damage");

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceW += Time.deltaTime;
        timeChanneled += Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), 4, qKeys[1]);
        BladeMark(QSkill().basic.name);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }


    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        if (!wCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
            BladeMark(WSkill().basic.name);
            timeSinceW = 0;
            timeChanneled = 0;

        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
            MyBuffManager.Add("StunBuff", new StunBuff(0.25f + (timeChanneled > 1 ? 1 : timeChanneled), TargetBuffManager, "StunBuff")); //what happens when channeling gets cancelled
            BladeMark(WSkill().basic.name);
            myStats.wCD = myStats.wSkill[0].basic.coolDown[4] - timeSinceW;
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        MyBuffManager.Add("AttackSpeed", new AttackSpeedBuff(8, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(4, eKeys[0], myStats, targetStats), "AttackSpeed"));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        float multiplier = RSkill().UseSkill(2, rKeys[0], myStats, targetStats) * 0.01f * myStats.PercentMissingHealth;
        UpdateAbilityTotalDamage(ref rSum, 3, new Damage(multiplier * 0.01f, SkillDamageType.Phyiscal), RSkill().basic.name);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        float multiplier = QSkill().UseSkill(4, qKeys[0], myStats, targetStats) * 0.01f;
        float damage = 0.2f * myStats.AD;
        if (targetStats.buffManager.buffs.TryGetValue("BladeOFRuinedKing", out Buff buff))
        {
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(damage, SkillDamageType.Phyiscal), QSkill().basic.name);
            AutoAttack(new Damage(myStats.AD * (1 + multiplier), SkillDamageType.Phyiscal));

            UpdateTotalHeal(ref qSum, damage * 1.35f, QSkill().basic.name);
        }
        else
        {
            AutoAttack(new Damage(myStats.AD * (1 + multiplier), SkillDamageType.Phyiscal));
        }
    }

    private void BladeMark(string source)
    {
        if (targetStats.buffManager.buffs.TryGetValue("BladeOfRuinedKing", out Buff buff))
            buff.duration = 4;
        else
            targetStats.buffManager.buffs.Add("BladeOfRuinedKing", new BladeOfRuinedKingBuff(4, TargetBuffManager, source));
    }
}