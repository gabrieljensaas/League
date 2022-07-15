using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChampStatus
{
    public bool stun;
    public float stunDuration;

    public bool silence;
    public float silenceDuration;

    public bool disarm;
    public float disarmDuration;

    public bool invincible;
    public float invincibleDuration;

    public bool damageReduction;
    public float damageReducFlat;
    public float damageReducPercent;
    public float damageReducDuration;

    void Update()
    {
        //Stun
        if (stun)
        {
            stunDuration -= Time.deltaTime;
            if (stunDuration <= 0)
            {
                stun = false;
            }
        }
        else
        {
            stunDuration = 0;
        }

        //Silence
        if (silence)
        {
            silenceDuration -= Time.deltaTime;
            if (silenceDuration <= 0)
            {
                silence = false;
            }
        }
        else
        {
            silenceDuration = 0;
        }

        //Disarm
        if (disarm)
        {
            disarmDuration -= Time.deltaTime;
            if (disarmDuration <= 0)
            {
                disarm = false;
            }
        }
        else
        {
            disarmDuration = 0;
        }

        //Invincible
        if (invincible)
        {
            invincibleDuration -= Time.deltaTime;
            if (invincibleDuration <= 0)
            {
                invincible = false;
            }
        }
        else
        {
            invincibleDuration = 0;
        }
    }
}
