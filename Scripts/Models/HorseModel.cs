using System;
using System.Collections.Generic;
using Godot;
using Horse.Scripts.Systems;

namespace Horse.Scripts.Models;

public class HorseModel
{   //
    public String Id {get; private set;} = Guid.NewGuid().ToString().Substring(0, 8);
    public String Name {get; set;}
    //default constant
    int DefaultMaxStat = 1200; int InitialStat = 100;

    //biological parameter
    public int Age {get; set;} = 0; 
    public int Nature{get; set;}
    public float GrowthRate{get; set;}
    public float DecayRate{get; set;}
    public int Peak{get; set;}

    //stats
    public int Spd {get; set;} public int MaxSpd {get; set;}
    public int Pow {get; set;} public int MaxPow {get; set;}
    public int Stam {get; set;} public int MaxStam {get; set;}

    //gene parameter
    public float GeneStability { get; set; }
    public List<GeneModel> GenesChain {get; set;} = new();

    public HorseModel() { }

    public HorseModel(String name, float stability, List<GeneModel> genes_chain)
    {
        Name = name; GeneStability = Math.Clamp(stability, 0.05f, 1.0f); 
        GenesChain = genes_chain;
    }

    public void PeakAgeCalc()
    {
        float baseAge = 30; //weeks
        float sensitivy = 0.4f;
        int tmp = (int)(baseAge * Mathf.Pow(GrowthRate/DecayRate, sensitivy));
        Peak = Math.Max(tmp, 24);
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
        Nature = Math.Clamp(Nature, 20, 100);
        GrowthRate = Math.Clamp(GrowthRate, 0.5f, 4.0f);
        DecayRate = Math.Clamp(DecayRate, 0.5f, 4.0f);
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
        int baseDecay = 100;
        int gain;
        if(Age <= Peak)
        {
            //calculate gain
            float ratio = (float)Age / Peak;
            gain = (int)(Nature * Mathf.Pow(ratio, GrowthRate));

            //stat calculate
            StatGain(gain);
        }
        else
        {
            //calculate gain
            float ratio = (float)(Age - Peak) / Peak;
            gain = (int)(Nature * Mathf.Exp(-DecayRate * ratio));

            //stat calculate
            StatGain(gain - baseDecay);
        }
        Age++;
    }
}