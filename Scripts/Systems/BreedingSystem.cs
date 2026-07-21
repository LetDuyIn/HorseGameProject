using System;
using System.Collections.Generic;
using Horse.Scripts.Models;

namespace Horse.Scripts.Systems;

public static class BreedingSystem
{
    private static readonly Random _random = new();
    public static HorseModel Breeding(HorseModel father, HorseModel mother, string foalName)
    {
        var genePool = new List<(GeneModel Gene, float ParentStability)>();
        foreach(var g in father.GenesChain) genePool.Add((g, father.GeneStability));
        foreach(var g in mother.GenesChain) genePool.Add((g, mother.GeneStability));

        float fatherStability = father.GeneStability;
        float motherStability = mother.GeneStability;
        float avgStability = (fatherStability + motherStability) / 2;

        float stabilityDrift = (float)(_random.NextDouble() * 0.16f - 0.08f);

        float foalStability = Math.Clamp(avgStability + stabilityDrift, 0.05f, 1.0f);

        List<GeneModel> foalGeneChain = new();
        int initalSlots = 4;

        while(foalGeneChain.Count < initalSlots && genePool.Count > 0)
        {
            var chosenGene = SelectGeneByStrength(genePool);
            float sourceStability = chosenGene.ParentStability;
            genePool.Remove(chosenGene);

            string newGeneId = GeneMutation(chosenGene.Gene.GeneId, sourceStability);

            float fluctatingRange = 0.1f * (1.0f - sourceStability);
            float foalGeneStr;
            float delta = (float)(_random.NextDouble() * fluctatingRange - (fluctatingRange / 2.0f));

            if(newGeneId == chosenGene.Gene.GeneId)
            {
                foalGeneStr = chosenGene.Gene.GeneStrength + delta;
            }
            else
            {
                float baseStr = GeneDatabase.Get(newGeneId).BaseGeneStrength;

                foalGeneStr = baseStr + delta;
            }

            foalGeneStr = Math.Clamp(foalGeneStr, 0.01f, 0.95f);

            foalGeneChain.Add(new GeneModel(newGeneId, foalGeneStr));
        }
        var foal = new HorseModel(foalName, foalStability, foalGeneChain);
        foal.ApplyGenetic();

        return foal;
    }

    private static (GeneModel Gene, float ParentStability) SelectGeneByStrength(List<(GeneModel Gene, float ParentStability)> pool)
    {
        float totalWeight = 0;
        foreach(var gene in pool)
        {
            totalWeight += gene.Gene.GeneStrength;
        }

        double roll = _random.NextDouble() * totalWeight;
        float curWeightSum = 0;

        foreach(var gene in pool)
        {
            curWeightSum += gene.Gene.GeneStrength;
            if(roll <= curWeightSum) return gene;
        }

        return pool[_random.Next(pool.Count)];
    }

    private static string GeneMutation(string geneId, float stability)
    {
        string[] parts = geneId.Split("-");
        string type = parts[0];
        string level = parts[1];

        int curLevel = int.Parse(level);

        int maxDelta = (int)Math.Ceiling((1.0 - stability) * 3);
        if(maxDelta == 0) return geneId;

        int delta = _random.Next(-maxDelta, maxDelta + 1);
        if(delta == 0) return geneId;

        int newLevel = curLevel + delta;
        if(newLevel < 0){newLevel = 0;};

       while(newLevel >= 0)
        {
            string candidateId = $"{type}-{newLevel:D2}";
            if(GeneDatabase.Get(candidateId) != null)
            {
                return candidateId;
            }
            newLevel--;
        }

        return geneId;
    }
}