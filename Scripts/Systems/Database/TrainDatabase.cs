using Horse.Scripts.Models;
using System.Collections.Generic;

namespace Horse.Scripts.Systems;

public static class TrainDatabase
{
    private static readonly Dictionary<string, TrainDefinition> _registry = new();

    static TrainDatabase()
    {
        Register(new TrainDefinition("Turf-00-01", 10, 10, 
            new() { {"SpdRatio", 0.1f},  {"StaRatio", 0.1f}, {"PowRatio", 0.1f}} ) );
    }

    private static void Register(TrainDefinition train)
    {
        _registry[train.TrainId] = train;
    }

    public static TrainDefinition Get(string trainId)
    {
        return _registry.TryGetValue(trainId, out var train) ? train : null;
    }
}