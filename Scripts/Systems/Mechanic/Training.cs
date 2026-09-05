using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Horse.Scripts.Models;

namespace Horse.Scripts.Systems;

public class TrainingSystem
{
    private static readonly Random _random = new();

    public static void Training(HorseModel horse, string trainId)
    {
        TrainDefinition train = TrainDatabase.Get(trainId);
        int energyCost = EnergyCalc(trainId);
        int horseEnergy = horse.Energy;
        int horseCon = horse.Con;

        if(IsInjured(horseEnergy, energyCost, horseCon) == true)
        {
            horse.Energy = Math.Max(0, horse.Energy - energyCost);
            horse.Injured = true;
            return;
        }

        float spread = 0.05f + (Math.Clamp(horse.Temperament, 0f, 1f) * 0.55f);
        float minTemp = 1.0f - spread;
        float maxTemp = 1.0f + spread;
        float tempMult = (float)(_random.NextDouble() * (maxTemp - minTemp) + minTemp);

        int baseGain = TrainDatabase.Get(trainId).Gain;
        float intMult = GetIntensity(trainId);
        float moodMult = horse.GetMoodMultiplier();
        float wisMult = horse.Wis;
        int gain = 0;

        float mult = intMult * moodMult * wisMult * tempMult;

        //spd gain
        gain = (int)(baseGain * mult * train.GetModByType("SpdRatio"));
        horse.Spd += gain;

        //sta gain
        gain = (int)(baseGain * mult * train.GetModByType("StaRatio"));
        horse.Stam += gain;

        //pow gain
        gain = (int)(baseGain * mult * train.GetModByType("PowRatio"));
        horse.Pow += gain;

        horse.Energy = Math.Max(0, horse.Energy - energyCost);
    }

    private static float GetIntensity(string trainId)
    {
        string[] parts = trainId.Split("-");
        int intensity = int.Parse(parts[2]);
        float val = 0.0f;
        
        switch (intensity)
        {
            case 1: 
                val = 1.1f;
                break;
            case 2:
                val = 1.3f;
                break;
            case 3:
                val = 1.5f;
                break;
        }

        return val;
    }

    private static int EnergyCalc(string trainId)
    {
        int baseCost = TrainDatabase.Get(trainId).Cost;
        float levelCostMult = 0.0f, intCostMult = 0.0f;

        string[] parts = trainId.Split("-");
        int level = int.Parse(parts[1]);
        int intensity = int.Parse(parts[2]);

        switch (level)
        {
            case 0:
                levelCostMult = 0.0f;
                break;
            case 1:
                levelCostMult = 1.1f;
                break;
            case 2:
                levelCostMult = 1.3f;
                break;
            case 3:
                levelCostMult = 1.6f;
                break;
            case 4:
                levelCostMult = 2.0f;
                break;
        }

        switch (intensity)
        {
            case 1:
                intCostMult = 1.1f;
                break;
            case 2:
                intCostMult = 1.3f;
                break;
            case 3:
                intCostMult = 1.6f;
                break;
        }

        int energyCost = (int)(baseCost * levelCostMult * intCostMult);
        return energyCost;
    }

    private static bool IsInjured(int curEnergy, int energyCost, int con)
    {
        if(curEnergy >= 50)
        {
            return false;
        }

        float lowEnergyGap = 50f - (float)curEnergy;
        float energyDeficit = (float)Math.Max(0, energyCost - curEnergy);
        float conResistance = (float)Math.Max(10f, con);
        float risk = (lowEnergyGap + (energyDeficit * 1.5f)) / conResistance;

        if (risk > 0.0f && _random.NextDouble() < risk)
        {
            return true;
        }

        return false;
    }
}