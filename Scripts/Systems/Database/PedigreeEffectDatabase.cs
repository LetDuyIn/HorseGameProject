using Horse.Scripts.Models;
using System.Collections.Generic;

namespace Horse.Scripts.Systems;

public static class PedigreeEffectDatabse
{
    private static readonly Dictionary<string, PedigreeEffectDefinition> _registry = new();

    private static void Register(PedigreeEffectDefinition effect)
    {
        _registry[effect.EffectId] = effect;
    }

    public static PedigreeEffectDefinition Get(string effectId)
    {
        return _registry.TryGetValue(effectId, out var effect) ? effect : null;
    }

    static PedigreeEffectDatabse()
    {
        Register(new PedigreeEffectDefinition("OutCross", 
            new() { {"InitSpd", 100},  {"InitStam", 100}, {"InitPow", 100}, {"Con", 100}, {"Wis", 100}} ) );
        
        Register(new PedigreeEffectDefinition("InBreeding", 
            new() { {"InitSpd", -100},  {"InitStam", -100}, {"InitPow", -100}, {"Con", -100}, {"Wis", -100}} ) );

        Register(new PedigreeEffectDefinition("LineBreeding-01", 
            new() { {"InitSpd", 100},  {"InitStam", 100}, {"InitPow", 100}} ) );

        Register(new PedigreeEffectDefinition("LineBreeding-02", 
            new() { {"InitSpd", 150},  {"InitStam", 125}, {"InitPow", 150}} ) );
        
        Register(new PedigreeEffectDefinition("MildInbreeding-01", 
            new() { {"InitSpd", 80},  {"InitStam", 70}, {"InitPow", 80}} ) );

        Register(new PedigreeEffectDefinition("MildInbreeding-02", 
            new() { {"InitSpd", 60},  {"InitStam", 30}, {"InitPow", 60}} ) );
    }
}