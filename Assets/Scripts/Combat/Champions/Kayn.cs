using Simulator.Combat;
using System.Collections;
using UnityEngine;

public enum KaynForm
{
    Neutral,
    Darkin,
    ShadowAssassin
}

public class Kayn : ChampionCombat
{
    public KaynForm kaynForm = KaynForm.Neutral;

    public static float ShadowAssassinBonus(int level) => 0.08f + (0.22f / 17 * (level - 1));
    public static float DarkinBonus(int level) => 0.2f + (0.1f / 17 * (level - 1));

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "A", "W", "E", "R" };

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
        checksE.Add(new CheckIfImmobilize(this));
        checksR.Add(new CheckIfImmobilize(this));

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        checkTakeDamagePostMitigation.Add(new CheckKaynFormBonus(this, this));

        qKeys.Add("Physical Damage");
        qKeys.Add("Total Physical Damage");

        wKeys.Add("Physical Damage");

        eKeys.Add("Duration");
        eKeys.Add("Heal");

        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }
    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Kayn's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if (kaynForm == KaynForm.Darkin)
        {
            UpdateTotalDamage(ref qSum, 0, new Damage((myStats.AD * 0.65f) + ((0.05f + (0.035f * (int)(myStats.AD / 100))) * targetStats.maxHealth), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)18560), QSkill().name);
            UpdateTotalDamage(ref qSum, 0, new Damage((myStats.AD * 0.65f) + ((0.05f + (0.035f * (int)(myStats.AD / 100))) * targetStats.maxHealth), SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)18560), QSkill().name);
        }
        else
		{
            UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)18560);
            UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)18560);
        }
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        if (kaynForm == KaynForm.ShadowAssassin)
            yield return new WaitForSeconds(0.55f);
        else if (kaynForm == KaynForm.Darkin)
        {
            TargetBuffManager.Add("Airborne", new AirborneBuff(1, TargetBuffManager, WSkill().name));
            yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        }
        else
            yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes:(SkillComponentTypes)18560);

        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        simulationManager.AddCastLog(myCastLog, 1);
    }

    //assume that heal will pop out for terrain
    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), ESkill().basic.name);

        UpdateTotalHeal(ref hSum, myStats.eSkill[0], myStats.eLevel, eKeys[1]);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));

        if (kaynForm == KaynForm.Darkin)
        {
            UpdateTotalDamage(ref rSum, 3, new Damage((0.15f + (0.13f * (int)(myStats.AD / 100))) * targetStats.maxHealth, SkillDamageType.Phyiscal, skillComponentType:(SkillComponentTypes)34944), RSkill().name);
            UpdateTotalHeal(ref hSum, 0.0975f + (0.0845f * (int)(myStats.AD / 100)), RSkill().name);
        }
        else
            UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)34944);
        MyBuffManager.Add("Untargetable", new UntargetableBuff(2.5f, myStats.buffManager, RSkill().name));

        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 4);
    }

    private class CheckKaynFormBonus : Check
    {
        private readonly Kayn kayn;

        public CheckKaynFormBonus(ChampionCombat ccombat, Kayn kayn) : base(ccombat)
        {
            this.kayn = kayn;
        }

        public override bool Control()
        {
            throw new System.NotImplementedException();
        }

        public override Damage Control(Damage damage)
        {
            if (kayn.kaynForm == KaynForm.Darkin)
                kayn.UpdateTotalHeal(ref kayn.hSum, damage.value * DarkinBonus(kayn.myStats.level), kayn.myStats.passiveSkill.skillName);
            else if (kayn.kaynForm == KaynForm.ShadowAssassin)
                kayn.UpdateTotalDamage(ref kayn.pSum, 4, new Damage(damage.value * ShadowAssassinBonus(kayn.myStats.level), SkillDamageType.Spell), kayn.myStats.passiveSkill.skillName);

            return damage;
        }
    }
}
