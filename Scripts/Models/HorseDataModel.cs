using System;
using System.Collections.Generic;
using Godot;

namespace Horse.Scripts.Models;

public class HorseDataModel
{
    public string Id{get; set;}
    public string Name{get; set;}
    public bool Gender;
    public List<Ancestor> SireLine = new();
    public List<Ancestor> DamLine = new();

    public int Age {get; set;} = 0; 
    public float Nature{get; set;}
    public float GrowthRate{get; set;} 
    public float DecayRate{get; set;} 
    public int Peak{get; set;}
    public float PeakFactor{get; set;}

    public int Spd {get; set;} public int MaxSpd {get; set;}
    public int Pow {get; set;} public int MaxPow {get; set;}
    public int Stam {get; set;} public int MaxStam {get; set;}

    public int Wis {get; set;}
    public int Con {get; set;}
    public int Energy {get; set;}
    public int MaxEnergy {get; set;}
    public bool Injured {get; set;}

    public Mood HorseMood {get; set;} 
    public float Temperament { get; set; }

    public float GeneStability { get; set; }
    public List<GeneModel> GenesChain {get; set;} = new();

    public HorseDataModel(){}

    public HorseDataModel(string id, string name, bool gender, List<Ancestor> sLine, List<Ancestor> dLine, List<GeneModel> geneChain)
    {
        this.Id = id;
        this.Name = name;
        this.Gender = gender;
        this.SireLine = sLine;
        this.DamLine = dLine;
        this.GenesChain = geneChain;
    }

    public string BestGeneId()
    {
        GeneModel best = new GeneModel();
        string id ="";
        foreach (GeneModel g in GenesChain)
        {
            if(g.GeneEvaluate() > best.GeneEvaluate())
            {
                id = g.GeneId;
            }
        }
        return id;
    }

}