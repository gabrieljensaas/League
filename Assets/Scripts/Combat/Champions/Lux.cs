using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Lux : ChampionCombat
{
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

        targetCombat.checksQ.Add(new CheckIfRooted(this));
        targetCombat.checksW.Add(new CheckIfRooted(this));
        targetCombat.checksE.Add(new CheckIfRooted(this));
        targetCombat.checksR.Add(new CheckIfRooted(this));
        targetCombat.checksA.Add(new CheckIfRooted(this));

        qKeys.Add("Magic Damage");
        wKeys.Add("Shield Strength");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        yield return base.ExecuteA();
        CheckIllumination();
    }

    public override IEnumerator ExecuteQ()
    {
        yield return base.ExecuteQ();
        targetStats.buffManager.buffs.Add("Root", new RootBuff(2, targetStats.buffManager, myStats.qSkill[0].basic.name));
        CheckIllumination();
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.shields.Add(myStats.wSkill[0].basic.name, new ShieldBuff(2.5f, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        yield return new WaitForSeconds(1);
        myStats.buffManager.buffs[myStats.wSkill[0].basic.name].duration = 2.5f;
        myStats.buffManager.buffs[myStats.wSkill[0].basic.name].value += myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats);
    }

    public override IEnumerator ExecuteE()
    {
        yield return base.ExecuteE();
        //TODO: Slow Debuff here
        CheckIllumination();
    }

    public override IEnumerator ExecuteR()
    {
        yield return base.ExecuteR();
        CheckIllumination();
    }

    private void CheckIllumination()
    {
        if (targetCombat.myStats.buffManager.buffs.TryGetValue("Illumination", out Buff illumination))
        {
            illumination.Kill();
            UpdateAbilityTotalDamage(ref pSum, 4, myStats.AD * Constants.GetLuxIlluminationByLevel(myStats.level, myStats.AP), myStats.passiveSkill.skillName, SkillDamageType.Spell);
        }
    }
}
