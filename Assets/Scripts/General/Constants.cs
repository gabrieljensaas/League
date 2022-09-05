using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static int[] ExpTable = { 0, 280, 660, 1140, 1720, 2400, 3180, 4060, 5040, 6120, 7300, 8580, 9960, 11440, 13020, 14700, 16480, 18360 };
    public static int MaxLevel = 18;
    public static float[] GarenEDamageByLevelTable = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 8.25f, 8.5f, 8.75f, 9, 9.25f, 9.5f, 9.75f, 10f, 10.25f };
    public static float[] AatroxPassiveCooldownByLevelTable = { 24, 23.29f, 22.59f, 21.88f, 21.18f, 20.47f, 19.76f, 19.06f, 18.35f, 17.65f, 16.94f, 16.24f, 15.53f, 14.82f, 14.12f, 13.41f, 12.71f, 12f};
    public static float[] TibbersEnragedAttackSpeeds = { 0.625f, 0.739f, 1.043f, 1.307f, 1.536f, 1.736f };
    public static float[] MasterYiWDamageReductionPercents = { 45f, 47.5f, 50f, 52.5f, 55f };
    public static float[] VayneRDurationBySkillLevel = { 8, 10, 12 };
    public static float[] VayneRBonusADBySkillLevel = { 25, 40, 55 };
    public static float[] VayneQCDReductionBySkillLevel = { 30, 40, 50 };
    public static float[] CaitlynTrapRechargeBySkillLevel = { 30, 24, 19, 15, 12 };
    public static float[] CaitlynMaxTrapBySkillLevel = { 3, 3, 4, 4, 5 };
    public static float[] MissFortuneRWaveIntervalTimeBySkillLevel = { 0.2036f, 0.1781f, 0.1583f};
    public static float[] MissFortuneRWaveCountBySkillLevel = { 14, 16, 18};
    public static float GetMissfortunePassiveADMultiplier(int level)
    {
        if (level < 4) return 0.5f;
        if (level < 7) return 0.6f;
        if (level < 9) return 0.7f;
        if (level < 11) return 0.8f;
        if (level < 13) return 0.9f;
        return 1;
    }
    public static float GetKaisaPassiveDamageByLevel(int level, int plasmaStacks, float AP)
    {
        if(level < 3) return 5 + (1 * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if(level < 4) return 8 + (1 * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if(level < 6) return 8 + (3.75f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if(level < 8) return 11 + (3.75f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if(level < 9) return 11 + (6.5f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if(level < 11) return 14 + (6.5f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if(level < 12) return 17 + (6.5f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if(level < 14) return 17 + (9.25f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if(level < 16) return 20 + (9.25f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        if(level < 17) return 20 + (9.25f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
        return 23 + (12f * plasmaStacks) + (AP * (15 + (2.5f * plasmaStacks) / 100));
    }

    public static float GetCaitlynPassivePercent(int level)
    {
        if (level < 7) return 60;
        if (level < 13) return 90;
        return 120;
    }
    public static float GetAnnieStunDurationByLevel(int level)
    {
        if (level < 6) return 1.25f;
        if (level < 11) return 1.5f;
        return 1.75f;
    }
    public static float GetRivenPassiveDamagePercentByLevel(int level)
    {
        if (level == 1) return 30f;
        if (level < 7) return 36f;
        if (level < 10) return 42f;
        if (level < 13) return 48f;
        if (level < 16) return 54f;
        return 60f;
    }
    public static int GetDariusNoxianMightByLevel(int level)
    {
        if (level < 10) return 30 + (5 * (level - 1));
        else if (level < 13) return 75 + (10 * (level - 10));
        else return 105 + (25 * (level - 13));
    }

    public static float GetDariusHemorrhageByLevel(int level, int stack) => (3f * stack) + (0.25f * stack * level);
    public static float GetDariusArmorReductionByLevel(int level) => 10 + (level * 5);
    public static float GetDariusNoxianGuillotineByLevel(int level, int stack)
    {
        if (level < 6) return 125 + (stack * .2f * 125);
        else if (level < 11) return 250 + (stack * .2f * 250);
        else return 375 + (stack * .2f * 375);
    }
}