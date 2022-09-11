using Simulator.Combat;

public class VayneAACheck : Check
{
    public VayneAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("Tumble", out Buff value))
        {
            damage += value.value;
            value.Kill();
        }

        if (combat.targetStats.buffManager.buffs.TryGetValue("SilverBolts", out Buff val))
        {
            if (val.value == 2)
            {
                val.Kill();
                float percentDamage = combat.myStats.wSkill[0].UseSkill(4, combat.wKeys[0], combat.myStats, combat.targetStats);
                float minDamage = combat.myStats.wSkill[0].UseSkill(4, combat.wKeys[1], combat.myStats, combat.targetStats);
                combat.UpdateAbilityTotalDamage(ref combat.wSum, 1, percentDamage > minDamage ? percentDamage : minDamage, combat.myStats.wSkill[0].basic.name, SkillDamageType.True);
            }
            else
            {
                val.value++;
                val.duration = 3.5f;
            }
        }
        else
        {
            combat.targetStats.buffManager.buffs.Add("SilverBolts", new SilverBoltsBuff(3.5f, combat.targetStats.buffManager, combat.myStats.passiveSkill.skillName));
        }

        if (combat.myStats.buffManager.buffs.TryGetValue("Untargetable", out Buff v))
        {
            v.Kill();
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}