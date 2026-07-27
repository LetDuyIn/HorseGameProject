using System.Collections.Generic;

namespace Horse.Scripts.Models;

public class GeneDefinition
{
    public string GeneId {get; set;}
    public float BaseGeneStrength {get; set;}
    public Dictionary<string, float> StatModifiers {get; set;} = new();

    public GeneDefinition(string id, float str, Dictionary<string, float> mod)
    {
        GeneId = id;
        BaseGeneStrength = str;
        StatModifiers = mod;
    }
}