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

        checkTakeDamageAA.Add(new CheckIfBlind(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Blind Duration");
        eKeys.Add("Magic Damage On-Hit");
        eKeys.Add("Magic Damage per Tick");
        eKeys.Add("Total Poison Damage");
        //rKeys.Add("Bounce Range");
        rKeys.Add("Maximum Charges");
        rKeys.Add("Magic Damage per Tick");
        rKeys.Add("Total Magic Damage");
        rKeys.Add("Slow");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        noxiousTrapTimer += Time.deltaTime;

        if (noxiousTrapTimer > noxiousTrapRechargeRate && noxiousTrapCharges < (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats))
        {
            noxiousTrapTimer = 0;
            noxiousTrapCharges++;
        }
    }

    public override IEnumerator ExecuteA()
    {
        yield return base.ExecuteA();

        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        if(targetStats.buffManager.buffs.TryGetValue("ToxicShot", out Buff buff))
            buff.duration = 4;
        else
            targetStats.buffManager.buffs.Add("ToxicShot", new ToxicShotBuff(4, targetStats.buffManager, myStats.eSkill[0].name));
    }

    public override IEnumerator ExecuteQ()
    {
        yield return base.ExecuteQ();
        targetStats.buffManager.buffs.Add("Blind", new BlindBuff((int)myStats.qSkill[0].UseSkill(4, qKeys[1], myStats, targetStats), targetStats.buffManager, myStats.qSkill[0].name));
    }

    public override IEnumerator ExecuteW()
    {
        yield return null;
    }

    public override IEnumerator ExecuteE()
    {
        yield return null;
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || targetStats.buffManager.buffs.ContainsKey("NoxiousTrap") || noxiousTrapCharges <= 0) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("NoxiousTrap", new NoxiousTrapBuff(4, targetStats.buffManager, myStats.eSkill[0].name, 1));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }
}
