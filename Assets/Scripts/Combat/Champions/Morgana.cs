using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Morgana : ChampionCombat
{
    private int timesProcW;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "W", "Q", "R", "E", "A" };

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

        qKeys.Add("Magic Damage");
        qKeys.Add("Root Duration");

        wKeys.Add("Minimum Damage Per Tick");

        eKeys.Add("Magic Shield Strength");

        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        TargetBuffManager.Add("Root", new RootBuff(QSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), TargetBuffManager, QSkill().basic.name));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes:SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile | SkillComponentTypes.Blockable, buffNames: new string[] { "Root" });
        //SoulSiphon heal
        UpdateTotalHeal(ref pSum, QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats) * 0.18f, myStats.passiveSkill.skillName);
        myStats.wCD -= 0.05f * WSkill().basic.coolDown[myStats.wLevel];
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW) || myStats.wLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        timesProcW = 0;
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        StartCoroutine(TormentedShadow());
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        MyBuffManager.Add("ShieldBuff", new ShieldBuff(5, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), "BlackShield", shieldType: ShieldBuff.ShieldType.Magic));
        MyBuffManager.Add("CCImmune", new ImmuneToCCBuff(5f, MyBuffManager, ESkill().basic.name, "BlackShield"));
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        //SoulSiphon heal
        UpdateTotalHeal(ref pSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats)* 0.18f, myStats.passiveSkill.skillName);
        myStats.wCD -= 0.05f * WSkill().basic.coolDown[myStats.wLevel];

        yield return new WaitForSeconds(3f);

        UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        //SoulSiphon heal
        UpdateTotalHeal(ref pSum, RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats)* 0.18f, myStats.passiveSkill.skillName);
        myStats.wCD -= 0.05f * WSkill().basic.coolDown[myStats.wLevel];

        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
    }

    public IEnumerator TormentedShadow()
    {
        if(timesProcW <= 10)
		{
            yield return new WaitForSeconds(0.5f);
            UpdateTotalDamage(ref wSum, 1, new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * targetStats.PercentMissingHealth * 0.017f, SkillDamageType.Spell), WSkill().basic.name);
            //SoulSiphon heal
            UpdateTotalHeal(ref pSum, WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) * targetStats.PercentMissingHealth * 0.017f * 0.18f, myStats.passiveSkill.skillName);
            myStats.wCD -= 0.05f * WSkill().basic.coolDown[myStats.wLevel];
            timesProcW++;
        }
    }
}