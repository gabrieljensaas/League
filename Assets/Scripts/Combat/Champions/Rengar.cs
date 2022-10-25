using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Rengar : ChampionCombat
{
    private int ferocityStacks = 0;
    private CheckRengarBattleRoar checkRengarBattleRoar;

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

        checkTakeDamage.Add(checkRengarBattleRoar);
        checkTakeDamage.Add(checkRengarBattleRoar);

        qKeys.Add("Bonus Physical Damage");
        qKeys.Add("Empowered Bonus Damage");

        wKeys.Add("Magic Damage");
        wKeys.Add("Empowered Damage");

        eKeys.Add("Physical Damage");
        eKeys.Add("Empowered Damage");

        rKeys.Add("Duration");
        rKeys.Add("Bonus Movement Speed");
        rKeys.Add("Armor Reduction");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        checkRengarBattleRoar.Update();
    }

    public override IEnumerator ExecuteA()
    {
        yield return base.ExecuteA();
        if (MyBuffManager.buffs.TryGetValue("SavageryBuff", out Buff buff))
            buff.value--;

        if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff thrillOfTheHunt))
        {
            if (ferocityStacks == 0) ferocityStacks++;
            TargetBuffManager.Add("ArmorReduction", new ArmorReductionBuff(4, TargetBuffManager, RSkill().name, RSkill().UseSkill(2, rKeys[2], myStats, targetStats), "ArmorReduction"));
            thrillOfTheHunt.Kill();
        }
    }

    public override IEnumerator ExecuteQ()
    {
        yield return base.ExecuteQ();
        attackCooldown = 0;
        MyBuffManager.Add("SavageryBuff", new SavageryBuff(3, MyBuffManager, QSkill().name));
        CheckFerocityStacks();
    }

    public override IEnumerator ExecuteW()
    {
        yield return base.ExecuteW();
        CheckFerocityStacks();
        UpdateTotalHeal(ref hSum, checkRengarBattleRoar.ReturnRecentDamageTaken() * 0.5f, WSkill().name);

        if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff buff))
            buff.Kill();
    }

    public override IEnumerator ExecuteE()
    {
        yield return base.ExecuteE();
        CheckFerocityStacks();

        if (MyBuffManager.buffs.TryGetValue("Untargetable", out Buff buff))
            buff.Kill();
    }

    public override IEnumerator ExecuteR()
    {
        yield return new WaitForSeconds(2);
        MyBuffManager.Add("Untargetable", new UntargetableBuff(RSkill().UseSkill(2, rKeys[1], myStats, targetStats), MyBuffManager, RSkill().name));
    }

    //assume that Ferocity never gets removed because we're never out of combat
    private void CheckFerocityStacks()
    {
        if (ferocityStacks < 4)
            ferocityStacks++;

        //auto assume that rengar will always use Empowered Savagery
        else
        {
            ferocityStacks = 0;
            UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[1]);
        }
    }
}
