using System.Collections;
using Simulator.Combat;
using UnityEngine;

public class Cassiopeia : ChampionCombat
{
    public bool isPoisoned = false;
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

        qKeys.Add("Magic Damage Per Tick");
        wKeys.Add("Magic Damage Per Tick");
        eKeys.Add("Bonus Magic Damage");
        eKeys.Add("Healing");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if (targetStats.buffManager.buffs.TryGetValue("PoisonBuff", out Buff buff))
            buff.duration = 3;
        else
            targetStats.buffManager.buffs.Add("PoisonBuff", new PoisonBuff(3, TargetBuffManager, "PoisonBuff"));
        myStats.qCD = QSkill().basic.coolDown[4];
        StartCoroutine(NoxiousBlast());
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        if (targetStats.buffManager.buffs.TryGetValue("PoisonBuff", out Buff buff))
            buff.duration = 5;
        else
            targetStats.buffManager.buffs.Add("PoisonBuff", new PoisonBuff(5, TargetBuffManager, "PoisonBuff"));
        myStats.wCD = WSkill().basic.coolDown[4];
        StartCoroutine(Miasma());
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        if(isPoisoned)
		{
            UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
            UpdateTotalHeal(ref eSum, ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), ESkill().basic.name);
        }
        UpdateAbilityTotalDamage(ref eSum, 2, 48 + 4 * myStats.level, ESkill().basic.name, SkillDamageType.Spell);
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        if (isPoisoned)
            MyBuffManager.Add("PetrifyingGaze", new StunBuff(2, TargetBuffManager, "PetrifyingGaze"));
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public IEnumerator NoxiousBlast()
    {
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
    }

    public IEnumerator Miasma()
    {
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        yield return new WaitForSeconds(1f);
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
    }
}
