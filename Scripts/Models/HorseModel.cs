using System;
using System.Collections.Generic;
using Horse.Scripts.Systems;

namespace Horse.Scripts.Models;

public class HorseModel
{   //
    public String Id {get; private set;} = Guid.NewGuid().ToString().Substring(0, 8);
    public String Name {get; set;}
    //
    int DefaultMaxStat = 1000; int InitialStat = 100;

    public int Spd {get; set;} public int MaxSpd {get; set;}
    public int Pow {get; set;} public int MaxPow {get; set;}
    public int Stam {get; set;} public int MaxStam {get; set;}

    public float GeneStability { get; set; }
    public List<GeneModel> GenesChain {get; set;} = new();

    public HorseModel() { }

    public HorseModel(String name, float stability, List<GeneModel> genes_chain)
    {
        Name = name; GeneStability = Math.Clamp(stability, 0.05f, 1.0f); 
        GenesChain = genes_chain;
    }

    public void ApplyGenetic()
    {
        MaxSpd = DefaultMaxStat; MaxPow = DefaultMaxStat; MaxStam = DefaultMaxStat;
        Spd = InitialStat; Pow = InitialStat; Stam = InitialStat;

        foreach(var gene in GenesChain)
        {
            var def = GeneDatabase.Get(gene.GeneId);

            foreach(var mod in def.StatModifiers)
            {
                switch (mod.Key)
                {
                    case "MaxSpd": MaxSpd += mod.Value; break;
                    case "InitSpd": Spd += mod.Value; break;
                    case "MaxPow": MaxPow += mod.Value; break;
                    case "InitPow": Pow += mod.Value; break;
                    case "MaxStam": MaxStam += mod.Value; break;
                    case "InitStam": Stam += mod.Value; break;
                }
            }
        }

        Spd = Math.Max(Spd, 0); MaxSpd = Math.Max(MaxSpd, 100);
        Pow = Math.Max(Pow, 0); MaxPow = Math.Max(MaxPow, 100);
        Stam = Math.Max(Stam, 0); MaxStam = Math.Max(MaxStam, 100);
    }
}