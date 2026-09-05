using System;
using System.Collections.Generic;
using Godot;
using Horse.Scripts.Systems;

namespace Horse.Scripts.Models;

public class HorseModel
{   
    private static readonly Random _random = new();
    //
    public String Id {get; private set;} = Guid.NewGuid().ToString().Substring(0, 8);
    public String Name {get; set;}
    //default constant
    int DefaultMaxStat = 1200; int InitialStat = 100;

    //biological parameter
    public int Age {get; set;} = 0; 
    public float Nature{get; set;}
    public float GrowthRate{get; set;} // neu < 1.0 -> tang nhanh roi cham khi gan peak; > 1.0 thi tang cham roi nhanh dan khi gan peak
    public float DecayRate{get; set;} // cang cao thi tut stat sau peak cang nhanh
    public int Peak{get; set;}
    public float PeakFactor{get; set;}

    //main stats
    public int Spd {get; set;} public int MaxSpd {get; set;}
    public int Pow {get; set;} public int MaxPow {get; set;}
    public int Stam {get; set;} public int MaxStam {get; set;}

    //training stats
    public int Wis {get; set;}
    public int Con {get; set;}
    public int Energy {get; set;}
    public int MaxEnergy {get; set;}
    public bool Injured {get; set;}

    //mood related stats
    public Mood HorseMood {get; set;} = Mood.Normal;
    public float Temperament { get; set; } = 0.5f;

    //gene parameter
    public float GeneStability { get; set; }
    public List<GeneModel> GenesChain {get; set;} = new();

    //function
    public HorseModel() { }

    public HorseModel(String name, float stability, List<GeneModel> genesChain)
    {
        Name = name; GeneStability = Math.Clamp(stability, 0.05f, 1.0f); 
        GenesChain = genesChain;
    }

    public void PeakAgeCalc()
    {
        float baseAge = 192; //weeks
        float sensitivity = 0.4f;
        int tmp = (int)(baseAge * Mathf.Pow(PeakFactor/DecayRate, sensitivity));
        Peak = tmp;
    }

    public void ApplyGenetic()
    {
        //stat calculate
        MaxSpd = DefaultMaxStat; MaxPow = DefaultMaxStat; MaxStam = DefaultMaxStat;
        Spd = InitialStat; Pow = InitialStat; Stam = InitialStat;

        foreach(var gene in GenesChain)
        {
            var def = GeneDatabase.Get(gene.GeneId);

            foreach(var mod in def.StatModifiers)
            {
                switch (mod.Key)
                {
                    //stats
                    case "MaxSpd": MaxSpd += (int)mod.Value; break;
                    case "InitSpd": Spd += (int)mod.Value; break;
                    case "MaxPow": MaxPow += (int)mod.Value; break;
                    case "InitPow": Pow += (int)mod.Value; break;
                    case "MaxStam": MaxStam += (int)mod.Value; break;
                    case "InitStam": Stam += (int)mod.Value; break;

                    //biological parameter
                    case "GRate": GrowthRate += mod.Value; break;
                    case "DRate": DecayRate += mod.Value; break;
                    case "Nat": Nature += (int)mod.Value; break;
                }
            }
        }

        //peak age calculate
        Nature = Math.Clamp((int)Nature, 1, 100);
        GrowthRate = Math.Clamp(GrowthRate, 0.5f, 4.0f);
        DecayRate = Math.Clamp(DecayRate, 0.5f, 4.0f);

        float minPeakFactor = DecayRate * 0.49f;
        PeakFactor = Math.Clamp(PeakFactor, minPeakFactor, 4.0f);
        PeakAgeCalc();

        //limit
        Spd = Math.Max(Spd, 0); MaxSpd = Math.Max(MaxSpd, 100);
        Pow = Math.Max(Pow, 0); MaxPow = Math.Max(MaxPow, 100);
        Stam = Math.Max(Stam, 0); MaxStam = Math.Max(MaxStam, 100);

    }

    public void StatGain(int gain)
    {
        Spd = Math.Clamp(Spd + gain, 0, MaxSpd);
        Pow = Math.Clamp(Pow + gain, 0, MaxPow);
        Stam = Math.Clamp(Stam + gain, 0, MaxStam);
    }

    public void Grow()
    {
        int baseDecay = 10;
        float gain;
        if(Age <= Peak)
        {
            //calculate gain
            float ratio = (float)Age / Peak;
            gain = (int)(Nature * Mathf.Pow(ratio, GrowthRate));

            //stat calculate
            StatGain((int)gain);
        }
        else
        {
            //calculate gain
            float ratio = (float)(Age - Peak) / Peak;
            gain = (-1) * (int)(Nature * Mathf.Exp(-DecayRate * ratio));

            //stat calculate
            StatGain((int)gain - baseDecay);
        }
        Age++;
    }

    public void UpdateMood()
    {
        float keepMoodChance = 1.0f - (Math.Clamp(Temperament, 0.0f, 1.0f) * 0.6f);

        double roll = _random.NextDouble();

        if (roll > keepMoodChance)
        {
            // 2. Nếu biến đổi: Ngẫu nhiên tăng (+1) hoặc giảm (-1) một bậc Mood
            int delta = _random.Next(0, 2) == 0 ? -1 : 1;
            int newMoodIndex = Math.Clamp((int)HorseMood + delta, 0, 4);

            HorseMood = (Mood)newMoodIndex;
        }
    }

    public float GetMoodMultiplier()
    {
        return HorseMood.GetMoodMult();
    }
}