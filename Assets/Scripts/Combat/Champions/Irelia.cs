using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Irelia : ChampionCombat
{
    public static float IonianFervorAttackSpeed(int level, int stack)
    {
        return level switch
        {
            < 7 => 7.5f * stack,
            < 13 => 13.75f * stack,
            _ => 20f * stack
        };
    }

    public static float IonianFervorEmpoweredDamage(int level) => 7 + (3 * level);

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "E", "W", "A" };

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

        qKeys.Add("Physical Damage");
        qKeys.Add("Heal");
        wKeys.Add("Minimum Physical Damage");
        wKeys.Add("Maximum Physical Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Cooldown Reduction");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if(UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)3994) != float.MinValue)
        {
            UpdateTotalDamage(ref qSum, 0, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)32768), QSkill().basic.name);
            UpdateTotalHeal(ref hSum, QSkill(), myStats.qLevel, qKeys[1]);
        }
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel] - (myStats.rLevel != -1 ? RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) : 0);
        IonianFervor(myStats.qSkill[0].name);

        if (targetStats.buffManager.buffs.TryGetValue("IonianFervorMark", out Buff buff))
        {
            myStats.qCD = 0.2f;
            buff.Kill();
        }
        simulationManager.AddCastLog(myCastLog, 0);
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: (SkillComponentTypes)18560);
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        IonianFervor(myStats.wSkill[0].name);
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), ESkill().basic.name);
        yield return new WaitForSeconds(0.4f); //second E cast and activation time
        if(UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)18560) != float.MinValue)
        {
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.75f, targetStats.buffManager, myStats.eSkill[0].name));
            UnsteadyMark(myStats.eSkill[0].name);
        }
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        IonianFervor(myStats.eSkill[0].name);
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        if(UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[1], skillComponentTypes: (SkillComponentTypes)18564) != float.MinValue)
            UnsteadyMark(myStats.rSkill[0].name);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];

        IonianFervor(myStats.rSkill[0].name);
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if(UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Irealia's Auto Attack") != float.MinValue)
        {
            if (myStats.buffManager.buffs.TryGetValue("IonianFervor", out Buff buff))
            {
                if (myStats.buffManager.buffs.TryGetValue("IonianFervorAS", out Buff ASbuff))
                    ((AttackSpeedBuff)ASbuff).KillSilent();

                MyBuffManager.Add("IonianFervorAS", new AttackSpeedBuff(6, myStats.buffManager, "IonianFervor", Irelia.IonianFervorAttackSpeed(myStats.level, (int)buff.value), "IonianFervorAS"));

                if (buff.value == 4)
                    UpdateTotalDamage(ref pSum, 4, new Damage(IonianFervorEmpoweredDamage(myStats.level) + myStats.bonusAD, SkillDamageType.Spell, skillComponentType: (SkillComponentTypes)32), "Ionian Fervor");
            }
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    private void IonianFervor(string source)
    {
        if (myStats.buffManager.buffs.TryGetValue("IonianFervor", out Buff buff))
        {
            if (buff.value < 4)
                buff.value++;

            buff.duration = 6;
        }
        else
            MyBuffManager.Add("IonianFervor", new IonianFervorBuff(6, myStats.buffManager, source));
    }

    private void UnsteadyMark(string source)
    {
        if (targetStats.buffManager.buffs.TryGetValue("IonianFervorMark", out Buff buff))
            buff.duration = 5;
        else
            targetStats.buffManager.buffs.Add("IonianFervorMark", new IonianFervorMarkBuff(6, targetStats.buffManager, source));
    }
}
