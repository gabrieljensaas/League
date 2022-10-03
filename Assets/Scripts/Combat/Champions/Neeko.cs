using Simulator.Combat;
using System.Collections;

public class Neeko : ChampionCombat
{
    public bool HasWPassive = false;
    private int wPassiveStack = 0;
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
        autoattackcheck = new NeekoAACheck(this, this);
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Total Maximum Magic Damage");
        wKeys.Add("Bonus Magic Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("Root Duration");
        rKeys.Add("Shield Strength");
        rKeys.Add("Bonus Shield Per Champion");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        MyBuffManager.Add(WSkill().basic.name, new UntargetableBuff(0.5f, MyBuffManager, WSkill().basic.name));
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        TargetBuffManager.Add("Root", new RootBuff(ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), TargetBuffManager, "Root"));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[2]);
        float totalShieldBuff = RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats) + RSkill().UseSkill(myStats.rLevel, rKeys[2], myStats, targetStats);
        MyBuffManager.Add("ShieldBuff", new ShieldBuff(2, MyBuffManager, RSkill().basic.name, totalShieldBuff, "ShieldBuff", shieldType: ShieldBuff.ShieldType.Normal));
        TargetBuffManager.Add("Stun", new StunBuff(1.25f, TargetBuffManager, "Stun"));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[2]);
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        wPassiveStack++;
    }
    public void NeekoWPassive()
    {
        if (wPassiveStack == 2)
        {
            HasWPassive = true;
        }
    }
}