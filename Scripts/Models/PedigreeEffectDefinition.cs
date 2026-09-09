using System;
using System.Collections.Generic;

namespace Horse.Scripts.Models;

public class PedigreeEffectDefinition
{
    public string EffectId {get; set;}
    public Dictionary<string, float> PedigreeModifiers {get; set;} = new();

    public PedigreeEffectDefinition()
    {
        
    }

    public PedigreeEffectDefinition(string id, Dictionary<string, float> mod)
    {
        this.EffectId = id;
        this.PedigreeModifiers = mod;
    }
}