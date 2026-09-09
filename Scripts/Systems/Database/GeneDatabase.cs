using System.Collections.Generic;
using System.Data;
using Horse.Scripts.Models;

namespace Horse.Scripts.Systems;

public static class GeneDatabase
{
    private static readonly Dictionary<string, GeneDefinition> _registry = new();

    static GeneDatabase()
    {
        //spd-oriented
        Register(new GeneDefinition("Spd-00", 0.1f, 
            new() { { "MaxSpd", -500 }, { "InitSpd", -200 }, { "MaxPow", -250 }, { "InitPow", -100 } }));

        Register(new GeneDefinition("Spd-01", 0.3f, 
            new() { { "MaxSpd", -250 }, { "InitSpd", -100 } }));

        Register(new GeneDefinition("Spd-02", 0.5f, 
            new() { { "MaxSpd", 100 }, { "InitSpd", 100 } }));

        Register(new GeneDefinition("Spd-03", 0.7f, 
            new() { { "MaxSpd", 300 }, { "InitSpd", 100 }, { "MaxPow", 100 } }));

        Register(new GeneDefinition("Spd-04", 0.65f, 
            new() { { "MaxSpd", 350 }, { "InitSpd", 200 }, { "MaxPow", 150 }, { "InitPow", 100 } }));

        Register(new GeneDefinition("Spd-05", 0.1f, 
            new() { { "MaxSpd", 500 }, { "InitSpd", 200 }, { "MaxPow", 200 }, { "InitPow", 150 } }));

        //power-oriented
        Register(new GeneDefinition("Pow-00", 0.1f, 
            new() { { "MaxPow", -500 }, { "InitPow", -200 }, { "MaxSpd", -250 }, { "InitSpd", -100 } }));

        Register(new GeneDefinition("Pow-01", 0.3f, 
            new() { { "MaxPow", -250 }, { "InitPow", -100 } }));

        Register(new GeneDefinition("Pow-02", 0.5f, 
            new() { { "MaxPow", 100 }, { "InitPow", 100 } }));

        Register(new GeneDefinition("Pow-03", 0.7f, 
            new() { { "MaxPow", 300 }, { "InitPow", 100 }, { "MaxStam", 100 } }));

        Register(new GeneDefinition("Pow-04", 0.65f, 
            new() { { "MaxPow", 350 }, { "InitPow", 200 }, { "MaxStam", 150 }, { "InitStam", 100 } }));

        Register(new GeneDefinition("Pow-05", 0.1f, 
            new() { { "MaxPow", 500 }, { "InitPow", 200 }, { "MaxSpd", 200 }, { "InitSpd", 150 } }));

        //stam-oriented
        Register(new GeneDefinition("Stam-00", 0.1f, 
            new() { { "MaxStam", -500 }, { "InitStam", -200 }, { "MaxPow", -250 }, { "InitPow", -100 } }));

        Register(new GeneDefinition("Stam-01", 0.3f, 
            new() { { "MaxStam", -250 }, { "InitStam", -100 } }));

        Register(new GeneDefinition("Stam-02", 0.5f, 
            new() { { "MaxStam", 100 }, { "InitStam", 100 } }));

        Register(new GeneDefinition("Stam-03", 0.7f, 
            new() { { "MaxStam", 300 }, { "InitStam", 100 }, { "MaxSpd", 100 } }));

        Register(new GeneDefinition("Stam-04", 0.65f, 
            new() { { "MaxStam", 350 }, { "InitStam", 200 }, { "MaxSpd", 150 }, { "InitSpd", 100 } }));

        Register(new GeneDefinition("Stam-05", 0.1f, 
            new() { { "MaxStam", 500 }, { "InitStam", 200 }, { "MaxPow", 200 }, { "InitPow", 150 } }));

        //hyper
        Register(new GeneDefinition("Hyper-01", 0.4f, 
            new() { { "MaxSpd", 80 }, { "MaxPow", 80 }, { "MaxStam", 80 } }));

        Register(new GeneDefinition("Hyper-02", 0.6f, 
            new() { { "MaxSpd", 150 }, { "InitSpd", 50 }, { "MaxPow", 150 }, { "InitPow", 50 }, { "MaxStam", 150 }, { "InitStam", 50 } }));

        Register(new GeneDefinition("Hyper-03", 0.05f, 
            new() { { "MaxSpd", 400 }, { "InitSpd", 250 }, { "MaxPow", 400 }, { "InitPow", 250 }, { "MaxStam", 400 }, { "InitStam", 250 } }));

        //biological stat
        Register(new GeneDefinition("Late-00", 0.5f, 
            new() { { "GRate",  3}, { "DRate", -1.5f } }));

        Register(new GeneDefinition("Early-00", 0.5f, 
            new() { { "GRate", -1.5f }, { "DRate", 3 } }));

        Register(new GeneDefinition("Longevity-00", 0.5f, 
            new() { {"DRate", 1} }));
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