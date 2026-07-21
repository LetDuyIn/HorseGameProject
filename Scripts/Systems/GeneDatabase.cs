using System.Collections.Generic;
using Horse.Scripts.Models;

namespace Horse.Scripts.Systems;

public static class GeneDatabase
{
    private static readonly Dictionary<string, GeneDefinition> _registry = new();

    static GeneDatabase()
    {
        //spd-oriented
        Register(new GeneDefinition("Spd-00", 0.1f, 
            new() {{"MaxSpeed", -500}, {"InitialSpeed", -200}, {"MaxPow", -250}, {"InitialPow", -100}}));

        Register(new GeneDefinition("Spd-01", 0.3f, 
            new() {{"MaxSpeed", -250}, {"InitialSpeed", -100}}));

        Register(new GeneDefinition("Spd-02", 0.5f, 
            new() {{"MaxSpeed", 100}, {"InitialSpeed", 100}}));

        Register(new GeneDefinition("Spd-03", 0.7f, 
            new() {{"MaxSpeed", 300}, {"InitialSpeed", 100}, {"MaxPow", 100}}));

        Register(new GeneDefinition("Spd-04", 0.65f, 
            new() {{"MaxSpeed", 350}, {"InitialSpeed", 200}, {"MaxPow", 150}, {"InitialPow", 100}}));

        Register(new GeneDefinition("Spd-05", 0.1f, 
            new() {{"MaxSpeed", 500}, {"InitialSpeed", 200}, {"MaxPow", 200}, {"InitialPow", 150}}));
    }

    private static void Register(GeneDefinition gene)
    {
        _registry[gene.GeneId] = gene;
    }

    public static GeneDefinition Get(string geneId)
    {
        return _registry.TryGetValue(geneId, out var gene) ? gene : null;
    }

}