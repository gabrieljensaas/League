using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class KhaZix : ChampionCombat
{
    public bool isQEvolved;
    public bool isREvolved;
    private bool isUnseenThreat = true;
    private int recastR = 2;
    private float recastTimer = 0;

    public static float UnseenThreatDamage(int level) => 8 + (6 * level);

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "W", "A" };

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

        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));

        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Physical damage");
        qKeys.Add("Increased Damage");

        wKeys.Add("Physical Damage");
        wKeys.Add("Heal");

        eKeys.Add("Physical Damage");

        if (isREvolved) recastR = 3;

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        recastTimer += Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteA()
    {
        yield return base.ExecuteA();

        if (isUnseenThreat)
        {
            UpdateTotalDamage(ref pSum, 4, new Damage(UnseenThreatDamage(myStats.level) + (myStats.bonusAD * 0.4f), SkillDamageType.Spell), myStats.passiveSkill.skillName);
            isUnseenThreat = false;
        }

        if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff buff))
            buff.Kill();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[1]);
        myStats.qCD = QSkill().basic.coolDown[4];

        if (isQEvolved) myStats.qCD *= 0.55f;

        if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff buff))
            buff.Kill();
    }

    public override IEnumerator ExecuteW()
    {
        yield return base.ExecuteW();
        UpdateTotalHeal(ref hSum, myStats.wSkill[0], 4, wKeys[1]);

        if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff buff))
            buff.Kill();
    }

    public override IEnumerator ExecuteE()
    {
        yield return base.ExecuteE();

        if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff buff))
            buff.Kill();
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        StartCoroutine(VoidAssaultCD());

        if (recastTimer >= 2)
        {
            recastTimer = 0;
            recastR--;
            MyBuffManager.Add("Untargetable", new UntargetableBuff(1.25f, MyBuffManager, RSkill().name));
            isUnseenThreat = true;
        }

        if (recastR == 0)
        {
            myStats.rCD = RSkill().basic.coolDown[2];
            StopCoroutine(VoidAssaultCD());
        }
    }

    private IEnumerator VoidAssaultCD()
    {
        yield return new WaitForSeconds(10);
        myStats.rCD = RSkill().basic.coolDown[2];
    }
}
