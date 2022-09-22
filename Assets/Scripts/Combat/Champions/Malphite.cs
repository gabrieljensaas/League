using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Malphite : ChampionCombat
{
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

        autoattackcheck = new MalphiteAACheck(this);
        checkTakeDamageAbility.Add(new CheckShield(this));
        checkTakeDamageAA.Add(new CheckShield(this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Slow");
        wKeys.Add("Bonus Armor");
        wKeys.Add("Increased Bonus Armor");
        wKeys.Add("Bonus Physical Damage");
        wKeys.Add("Physical Damage");
        eKeys.Add("Magic Damage");
        eKeys.Add("Cripple");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();

        //didn't add shield recovery because constant fighting anyways
        myStats.buffManager.buffs.Add("GraniteShield", new ShieldBuff(999f, myStats.buffManager, "Granite Shield", myStats.maxHealth * 0.09f, "GraniteShield"));
        //preset to 90% armor buff because of 
        myStats.buffManager.buffs.Add("ThunderclapArmor", new ArmorBuff(999f, myStats.buffManager, "Thunderclap", myStats.armor * 0.9f, "ThunderclapArmor"));
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        if(!myStats.buffManager.buffs.ContainsKey("GraniteShield") && !myStats.buffManager.buffs.ContainsKey("ThunderclapArmor")) //replace tripled armor with base bonus
            myStats.buffManager.buffs.Add("ThunderclapArmor", new ArmorBuff(999f, myStats.buffManager, "Thunderclap", myStats.armor * 0.3f, "ThunderclapArmor"));
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("ThunderclapBuff", new ThunderclapBuff(5, myStats.buffManager, myStats.wSkill[0].name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        myStats.MyCombat.attackCooldown = 0;
    }

    public override IEnumerator ExecuteE()
    {
        yield return base.ExecuteE();
        targetStats.buffManager.buffs.Add("Cripple", new CrippleBuff(3, targetStats.buffManager, myStats.eSkill[0].name, myStats.eSkill[0].UseSkill(4, eKeys[1], myStats, targetStats)));
    }

    public override IEnumerator ExecuteR()
    {
        yield return base.ExecuteR();
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(1.5f, targetStats.buffManager, myStats.rSkill[0].name));
    }
}
