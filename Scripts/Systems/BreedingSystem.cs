using System;
using System.Collections.Generic;
using Horse.Scripts.Models;

namespace Horse.Scripts.Systems;

public static class BreedingSystem
{
    private static readonly Random _random = new();
    public static HorseModel Breeding(HorseModel father, HorseModel mother, string foalName)
    {   
        //const
        float maxCapStability = 0.95f;
        float minCapStability = 0.05f;
        float bonusGeneCost = 0.1f;
        //gene pool build
        var genePool = new List<GeneModel>();
        foreach(var g in father.GenesChain) genePool.Add(g);
        foreach(var g in mother.GenesChain) genePool.Add(g);

        //foal stability calc
        float avgStability = (father.GeneStability + mother.GeneStability) / 2;
        float stabilityDrift = (float)(_random.NextDouble() * 0.16f - 0.08f);
        float foalStability = Math.Clamp(avgStability + stabilityDrift, minCapStability, maxCapStability);

        //foal biological calc
        float avgNature = (father.Nature + mother.Nature) / 2;
        float natFactor = (float)(1.0f + (_random.NextDouble() * 0.2f - 0.1f) * (1.0f - foalStability));
        float foalNature = avgNature * natFactor;

        float avgGRate = (father.GrowthRate + mother.GrowthRate) / 2;
        float gRateDrift = (float)(_random.NextDouble() * 0.2f - 0.1f); // Dịch chuyển độc lập
        float foalGRate = avgGRate + gRateDrift;

        float avgDRate = (father.DecayRate + mother.DecayRate) / 2;
        float dRateDrift = (float)(_random.NextDouble() * 0.2f - 0.1f); // Dịch chuyển độc lập
        float foalDRate = avgDRate + dRateDrift;

        float avgPeakFactor = (father.PeakFactor + mother.PeakFactor) / 2;
        float pFactorDrift = (float)(_random.NextDouble() * 0.2f - 0.1f); // Dịch chuyển độc lập
        float foalPFactor = avgPeakFactor + pFactorDrift;

        //foal gene chain build
        List<GeneModel> foalGeneChain = new();
        int initalSlots = 4;
        while(foalGeneChain.Count < initalSlots && genePool.Count > 0)
        {
            AddGeneProcess(genePool, foalGeneChain, foalStability);
        }

        while(genePool.Count > 0)
        {
            float roll = (float)_random.NextDouble();
            if(roll <= foalStability)
            {
                AddGeneProcess(genePool, foalGeneChain,foalStability);
                foalStability -= bonusGeneCost;
            }
            else
            {
                break;
            }
        }

        //foal gene chain stability synchronize
        foalStability = Math.Clamp(foalStability, minCapStability, maxCapStability);
        foreach (var gene in foalGeneChain)
        {
            gene.GeneStability = foalStability;
        }
        
        //new foal
        var foal = new HorseModel(foalName, foalStability, foalGeneChain);
        foal.Nature = foalNature; foal.GrowthRate = foalGRate; foal.DecayRate = foalDRate; foal.PeakFactor = foalPFactor;
        foal.ApplyGenetic();
        return foal;
    }

    private static void AddGeneProcess(List<GeneModel> genePool, List<GeneModel> foalGeneChain, float stability)
    {
        var chosenGene = SelectGeneByStrength(genePool);
        float sourceStability = chosenGene.GeneStability;
        genePool.Remove(chosenGene);

        string newGeneId = GeneMutation(chosenGene.GeneId, sourceStability);

        float fluctatingRange = 0.1f * (1.0f - sourceStability);
        float foalGeneStr;
        float delta = (float)(_random.NextDouble() * fluctatingRange - (fluctatingRange / 2.0f));

        if(newGeneId == chosenGene.GeneId)
        {
            foalGeneStr = chosenGene.GeneStrength + delta;
        }
        else
        {
            float baseStr = GeneDatabase.Get(newGeneId).BaseGeneStrength;
            float inheritanceWeight = 0.3f;

            foalGeneStr = (1.0f - inheritanceWeight) * baseStr + inheritanceWeight * chosenGene.GeneStrength + delta;
        }

        foalGeneStr = Math.Clamp(foalGeneStr, 0.01f, 0.95f);

        foalGeneChain.Add(new GeneModel(newGeneId, foalGeneStr, stability));
    }

    private static GeneModel  SelectGeneByStrength(List<GeneModel> pool)
    {
        float totalWeight = 0;
        foreach(var gene in pool)
        {
            totalWeight += gene.GeneStrength;
        }

        double roll = _random.NextDouble() * totalWeight;
        float curWeightSum = 0;

        foreach(var gene in pool)
        {
            curWeightSum += gene.GeneStrength;
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