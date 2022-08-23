using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Pet
{
    protected ChampionCombat owner;

    protected Pet(ChampionCombat owner)
    {
        this.owner = owner;
    }

    public abstract void Update();
}