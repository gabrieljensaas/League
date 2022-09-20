using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Aatrox : ChampionCombat
{
    public static float[] AatroxPassiveCooldownByLevelTable = { 24, 23.29f, 22.59f, 21.88f, 21.18f, 20.47f, 19.76f, 19.06f, 18.35f, 17.65f, 16.94f, 16.24f, 15.53f, 14.82f, 14.12f, 13.41f, 12.71f, 12f };
    public float pCD = 0;
    private int qCounter = 0;
    private float timeSinceLastQ = 0f;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "W", "A", "" };

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
        autoattackcheck = new AatroxAACheck(this, this);
        targetCombat.checkTakeDamageAAPostMitigation.Add(new CheckIfEnemyHasDeathBringerStance(targetCombat, this));
        targetCombat.checkTakeDamageAAPostMitigation.Add(new CheckForAatroxEHeal(targetCombat, this));
        targetCombat.checkTakeDamageAbilityPostMitigation.Add(new CheckForAatroxEHeal(targetCombat, this));

        qKeys.Add("First Sweetspot Damage");
        qKeys.Add("Second Sweetspot Damage");
        qKeys.Add("Third Sweetspot Damage");
        wKeys.Add("Physical Damage");
        eKeys.Add("Healing");
        eKeys.Add("World Ender Increased Healing");
        rKeys.Add("Bonus Attack Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        timeSinceLastQ += Time.deltaTime;
        if (timeSinceLastQ > 4) qCounter = 0;
        pCD -= Time.deltaTime;
        if (pCD < 0 && !myStats.buffManager.buffs.ContainsKey("DeathbringerStance")) myStats.buffManager.buffs.Add("DeathbringerStance", new DeathbringerStanceBuff(float.MaxValue, myStats.buffManager, "Deathbringer Stance"));
    }

    public override IEnumerator ExecuteQ()
    {
        if (qCounter == 0 && myStats.qCD > 0) yield break;
        if (timeSinceLastQ < 1f) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if (qCounter == 0)
        {
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
            timeSinceLastQ = 0f;
            qCounter++;
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.25f, targetStats.buffManager, myStats.qSkill[0].basic.name));
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        }
        else if (qCounter == 2)
        {
            timeSinceLastQ = 0f;
            qCounter = 0;
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.25f, targetStats.buffManager, myStats.qSkill[0].basic.name));
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[2]);
        }
        else
        {
            timeSinceLastQ = 0f;
            qCounter++;
            targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.25f, targetStats.buffManager, myStats.qSkill[0].basic.name));
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[1]);
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(1.5f);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.25f, targetStats.buffManager, myStats.qSkill[0].basic.name));  //pulled airborne needs research
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new AttackDamageBuff(10, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), myStats.rSkill[0].basic.name));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new AttackDamageBuff(10, targetStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(skillLevel, rKeys[0], targetStats, myStats), myStats.rSkill[0].basic.name));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
    }
}