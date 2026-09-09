using System;
using System.Collections.Generic;
using Horse.Scripts.Models;

namespace Horse.Scripts.Systems;

public static class BreedingSystem
{
    private static readonly Random _random = new();
    public static HorseModel Breeding(HorseDataModel father, HorseDataModel mother, string foalName)
    {   
        List<Ancestor> sireLine = new();
        sireLine.AddRange(PedigreeSystem.PedigreeLineBuild(father));
        List<Ancestor> damLine = new();
        damLine.AddRange(PedigreeSystem.PedigreeLineBuild(mother));

        List<(string id, int sGen, int dGen)> overlapData = PedigreeSystem.PedigreeOverlap(sireLine, damLine);
        PedigreeType pedigreeType = PedigreeSystem.PedigreeTypeCheck(overlapData);

        float bonusGeneCost = 0.0f;
        
        switch (pedigreeType)
        {
            case PedigreeType.OutCross:
                bonusGeneCost = 0.01f;
                break;
            case PedigreeType.LineBreeding:
                bonusGeneCost = 0.3f;
                break;
            case PedigreeType.MildInbreeding:
                bonusGeneCost = 0.08f;
                break;
            case PedigreeType.Inbreeding:
                bonusGeneCost = 0.1f;
                break;
        }

        //const
        float maxCapStability = 0.95f;
        float minCapStability = 0.05f;

        //foal stability calc
        float avgStability = (father.GeneStability + mother.GeneStability) / 2;
        float stabilityDrift = (float)(_random.NextDouble() * 0.16f - 0.08f);
        float foalStability = Math.Clamp(avgStability + stabilityDrift, minCapStability, maxCapStability);

        //foal biological calc
        float foalNature  = BaseParentInherited(father.Nature, mother.Nature, foalStability, 0.2f);
        float foalGRate   = BaseParentInherited(father.GrowthRate, mother.GrowthRate, foalStability, 0.15f);
        float foalDRate   = BaseParentInherited(father.DecayRate, mother.DecayRate, foalStability, 0.15f);
        float foalPFactor = BaseParentInherited(father.PeakFactor, mother.PeakFactor, foalStability, 0.15f);
        

        //foal gene chain build
        List<GeneModel> foalGeneChain = new();

        if (pedigreeType != PedigreeType.OutCross)
        {
            List<GeneModel> pedigreeGenes = PedigreeSystem.AncestorGeneExtract(overlapData);
            
            foreach (var pGene in pedigreeGenes)
            {
                foalGeneChain.Add(pGene);
            }
        }

        var genePool = new List<GeneModel>();
        foreach(var g in father.GenesChain) genePool.Add(g);
        foreach(var g in mother.GenesChain) genePool.Add(g);


        int totalInitialSlots = 4;
        int remainingInitialSlots = Math.Max(0, totalInitialSlots - foalGeneChain.Count);
        while(remainingInitialSlots > 0 && genePool.Count > 0)
        {
            AddGeneProcess(genePool, foalGeneChain, foalStability);
            remainingInitialSlots--;
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
        HorseModel foal = new HorseModel(foalName, foalStability, foalGeneChain)
        {
            Nature = foalNature,
            GrowthRate = foalGRate,
            DecayRate = foalDRate,
            PeakFactor = foalPFactor,
            SireLine = sireLine,
            DamLine = damLine

        };
        
        PedigreeSystem.PedigreeEffect(foal, pedigreeType, overlapData.Count);

        foal.ApplyGenetic();
        return foal;
    }

    private static void AddGeneProcess(List<GeneModel> genePool, List<GeneModel> foalGeneChain, float stability)
    {
        if(genePool.Count == 0) return;
        
        var chosenGene = SelectGeneByStrength(genePool);
        float sourceStability = chosenGene.GeneStability;
        genePool.Remove(chosenGene);

        string newGeneId = GeneMutation(chosenGene.GeneId, sourceStability);

        float fluctuatingRange = 0.1f * (1.0f - sourceStability);
        float foalGeneStr;
        float delta = (float)(_random.NextDouble() * fluctuatingRange - (fluctuatingRange / 2.0f));

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

    private static GeneModel SelectGeneByStrength(List<GeneModel> pool)
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

    private static float BaseParentInherited(float f, float m, float stability, float varianceRange = 0.1f)
    {
        float avg = (f + m) / 2;
        float actualRange = varianceRange * (1.0f - stability);
        float drift = 1.0f + (float)(_random.NextDouble() * (actualRange * 2) - actualRange);

        return avg * drift;
    }
}