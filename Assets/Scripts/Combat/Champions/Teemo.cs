using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Teemo : ChampionCombat
{
    private int noxiousTrapCharges = 3; //hard coded for now
    private float noxiousTrapTimer = 0;
    private float noxiousTrapRechargeRate = 14; //hard coded for now

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "A", "Q", "", "" };

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

        checkTakeDamage.Add(new CheckIfBlind(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Blind Duration");

        eKeys.Add("Magic Damage On-Hit");
        eKeys.Add("Magic Damage per Tick");

        rKeys.Add("Maximum Charges");
        rKeys.Add("Magic Damage per Tick");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        noxiousTrapTimer += Time.fixedDeltaTime;

        if (myStats.rLevel == -1) return;
        if (noxiousTrapTimer > noxiousTrapRechargeRate && noxiousTrapCharges < (int)RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats))
        {
            noxiousTrapTimer = 0;
            noxiousTrapCharges++;
        }
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Teemo's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);

        UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes:(SkillComponentTypes) 40);
        if (TargetBuffManager.buffs.TryGetValue("ToxicShot", out Buff buff))
            buff.duration = 4;
        else
            TargetBuffManager.Add("ToxicShot", new ToxicShotBuff(4, TargetBuffManager, ESkill().name));
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)34948);
        TargetBuffManager.Add("Blind", new BlindBuff((int)QSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), TargetBuffManager, QSkill().name));
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteE()
    {
        yield return null;
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR) || TargetBuffManager.buffs.ContainsKey("NoxiousTrap") || noxiousTrapCharges <= 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        TargetBuffManager.Add("NoxiousTrap", new NoxiousTrapBuff(5, TargetBuffManager, RSkill().name, 1));
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 4);
    }
}
