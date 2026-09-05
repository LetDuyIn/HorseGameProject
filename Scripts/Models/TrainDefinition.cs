using System;
using System.Collections.Generic;

namespace Horse.Scripts.Models;

public class TrainDefinition
{
    public string TrainId {get; set;}
    public Dictionary<string, float> TrainModifiers {get; set;} = new();

    public int Cost {get; set;}
    public int Gain {get; set;}

    public TrainDefinition(String id, int cost, int gain, Dictionary<string, float> mod)
    {
        this.TrainId = id;
        this.Cost = cost;
        this.Gain = gain;
        TrainModifiers = mod;
    }

    public float GetModByType(string type)
    {
        switch (type)
        {
            case "SpdRatio": return TrainModifiers["SpdRatio"]; break;
            case "StaRatio": return TrainModifiers["StaRatio"]; break;
            case "PowRatio": return TrainModifiers["PowRatio"]; break;
        }
        return 1.0f;
    }
}