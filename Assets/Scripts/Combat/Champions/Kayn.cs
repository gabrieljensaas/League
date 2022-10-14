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

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        checkPostMitigationDamageAbility.Add(new CheckKaynFormBonus(this, this));

        qKeys.Add("Physical Damage");
        qKeys.Add("Total Physical Damage");

        wKeys.Add("Physical Damage");

        eKeys.Add("Duration");
        eKeys.Add("Heal");

        rKeys.Add("Physical Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if (kaynForm == KaynForm.Darkin)
        {
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage((myStats.AD * 0.65f) + ((0.05f + (0.035f * (int)(myStats.AD / 100))) * targetStats.maxHealth), SkillDamageType.Phyiscal), QSkill().name);
        }
        else
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[1]);
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
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
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);

        myStats.wCD = WSkill().basic.coolDown[4];
    }

    //assume that heal will pop out for terrain
    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateTotalHeal(ref hSum, myStats.eSkill[0], 4, eKeys[1]);
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));

        if (kaynForm == KaynForm.Darkin)
        {
            UpdateAbilityTotalDamage(ref rSum, 0, new Damage((0.15f + (0.13f * (int)(myStats.AD / 100))) * targetStats.maxHealth, SkillDamageType.Phyiscal), RSkill().name);
            UpdateTotalHeal(ref hSum, 0.0975f + (0.0845f * (int)(myStats.AD / 100)), RSkill().name);
        }
        else
            UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        myStats.buffManager.buffs.Add("Untargetable", new UntargetableBuff(2.5f, myStats.buffManager, RSkill().name));

        myStats.rCD = RSkill().basic.coolDown[2];
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
                kayn.UpdateAbilityTotalDamage(ref kayn.pSum, 4, new Damage(damage.value * ShadowAssassinBonus(kayn.myStats.level), SkillDamageType.Spell), kayn.myStats.passiveSkill.skillName);

            return damage;
        }
    }
}
