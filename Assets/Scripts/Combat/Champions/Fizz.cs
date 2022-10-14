using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Fizz : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "E", "Q", "A" };

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
        checkTakeDamageAA.Add(new CheckForNimbleFighter(this));
        checkTakeDamageAbility.Add(new CheckForNimbleFighter(this));
        checksQ.Add(new CheckIfUnableToAct(this));
        checksW.Add(new CheckIfUnableToAct(this));
        checksE.Add(new CheckIfUnableToAct(this));
        checksR.Add(new CheckIfUnableToAct(this));
        checksA.Add(new CheckIfUnableToAct(this));
        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage per Tick");
        wKeys.Add("Bonus Magic Damage");
        wKeys.Add("Bonus Magic Damage On-Hit");
        eKeys.Add("Magic Damage");
        rKeys.Add("Guppy Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if (UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Dash) != float.MinValue)  //needs to be countered by parry too but could not find a way to implement that
        {
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(myStats.AD, SkillDamageType.Phyiscal), QSkill().basic.name);   // will apply on hit effects but since it can be countered by spell shield didnt add that as skillcomponenttype
        }
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        MyBuffManager.Add("SeastoneTrident", new SeastoneTridentBuff(4, MyBuffManager, WSkill().basic.name));
        //UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()                                      //min wait time for now
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        MyBuffManager.Add("Untargetable", new UntargetableBuff(0.65f, MyBuffManager, ESkill().basic.name));
        MyBuffManager.Add("UnableToAct", new UnableToActBuff(0.65f, MyBuffManager, ESkill().basic.name));
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Spell, SkillComponentTypes.Dash), ESkill().basic.name);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(0.65f);
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
    }

    public override IEnumerator ExecuteR()                               //min range for now
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        StartCoroutine(ChumpOfTheWaters());
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if (AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal, SkillComponentTypes.OnHit)).damage != float.MinValue)
        {
            StopCoroutine(SeastoneTridentPassive(3f));
            StartCoroutine(SeastoneTridentPassive(3f));

            if (MyBuffManager.buffs.TryGetValue("SeastoneTrident", out Buff buff))
            {
                if (buff.value == 1)
                {
                    buff.duration = 5f;
                    buff.value = 0;
                    UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[1]);
                }
                else
                {
                    UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[2], skillComponentTypes: SkillComponentTypes.ProcDamage);
                }
            }
        }
    }

    public IEnumerator SeastoneTridentPassive(float timeRemaning)
    {
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: SkillComponentTypes.PersistentDamage);
        if (timeRemaning - 0.5f > 0) StartCoroutine(SeastoneTridentPassive(timeRemaning - 0.5f));
    }

    public IEnumerator ChumpOfTheWaters()
    {
        yield return new WaitForSeconds(2f);
        TargetBuffManager.Add("Airborne", new AirborneBuff(1f, TargetBuffManager, RSkill().basic.name));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], buffNames: new string[] { "Airborne" });
    }
}