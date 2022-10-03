using Simulator.Combat;
using System.Collections.Generic;
using UnityEngine;



public class CheckRengarBattleRoar : Check
{
    private List<DamageInstance> damageInstance;

    public CheckRengarBattleRoar(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        damageInstance.Add(new DamageInstance { damage = damage });
        return damage;
    }

    public void Update()
    {
        foreach(DamageInstance damage in damageInstance)
        {
            damage.time += Time.deltaTime;
            if (damage.time > 1.5f) damageInstance.Remove(damage);
        }
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }

    public float ReturnRecentDamageTaken()
    {
        float totalDamage = 0;
        foreach (DamageInstance damage in damageInstance)
            totalDamage += damage.damage;

        return totalDamage;
    }

    private sealed class DamageInstance
    {
        public float time;
        public float damage;
    }
}