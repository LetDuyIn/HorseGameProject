using System;
using System.Collections.Generic;

using Godot;
using Horse.Scripts.Models;
using Horse.Scripts.Systems;

namespace Horse.Scripts.Systems;

public enum PedigreeType
{
    OutCross,
    LineBreeding,
    MildInbreeding,
    Inbreeding
}

public class PedigreeSystem
{
    public static List<Ancestor> PedigreeLineBuild(HorseDataModel parent)
    {
        List<Ancestor> ancestors = new List<Ancestor>();
        ancestors.Add(new Ancestor(parent.Id, 1));

        foreach(Ancestor a in parent.SireLine)
        {
            if (a.Generation + 1 <= 4)
            {
                ancestors.Add(new Ancestor(a.Id, a.Generation + 1));
            }
        }
        foreach(Ancestor a in parent.DamLine)
        {
            if (a.Generation + 1 <= 4)
            {
                ancestors.Add(new Ancestor(a.Id, a.Generation + 1));
            }
        }
        return ancestors;
    }

    

    public static List<(String, int, int)> PedigreeOverlap(List<Ancestor> sLine, List<Ancestor> dLine)
    {
        List<(String id, int sGen, int dGen)> data = new();
        foreach(Ancestor aS in sLine)
        {
            foreach(Ancestor aD in dLine)
            {
                if(aS.Id == aD.Id)
                {
                    data.Add((aS.Id, aS.Generation, aD.Generation));
                }
            }
        }
        return data;
    }

    public static PedigreeType PedigreeTypeCheck(List<(String, int, int)> data)
    {
        if(data.Count == 0)
        {
            return PedigreeType.OutCross;
        }
        Dictionary<string, float> riskMap = new();
        
        float highestRisk = 0.0f;

        foreach((String id, int sGen, int dGen) commonCross in data)
        {
            string id = commonCross.id;

            float contribution = Mathf.Pow(0.5f, commonCross.sGen + commonCross.dGen - 1);

            if (riskMap.ContainsKey(id))
            {
                riskMap[id] += contribution;
            }
            else
            {
                riskMap[id] = contribution;
            }

            if (riskMap[id] > highestRisk)
            {
                highestRisk = riskMap[id];
            }

        }

        if (highestRisk <= 0.0156f) return PedigreeType.LineBreeding;    
        if (highestRisk <= 0.0625f) return PedigreeType.MildInbreeding;  
        return PedigreeType.Inbreeding;                                  
    }

    public static string EffectIdGenerate(string type, int level)
    {
        string effectId = $"{type}-{level:D2}";
        return effectId;
    }

    public static void ApplyPedigreeFlatEffect(HorseModel foal, string effectId)
    {
        PedigreeEffectDefinition effect = PedigreeEffectDatabse.Get(effectId);
        foreach(var mod in effect.PedigreeModifiers)
        {
            switch (mod.Key)
            {
                    case "MaxSpd": foal.MaxSpd += (int)mod.Value; break;
                    case "InitSpd": foal.ModifySpd((int)mod.Value); break;
                    case "MaxPow": foal.MaxPow += (int)mod.Value; break;
                    case "InitPow": foal.ModifyPow((int)mod.Value); break;
                    case "MaxStam": foal.MaxStam += (int)mod.Value; break;
                    case "InitStam": foal.ModifyStam((int)mod.Value); break;

                    //biological parameter
                    case "GRate": foal.GrowthRate += mod.Value; break;
                    case "DRate": foal.DecayRate += mod.Value; break;
                    case "Nat": foal.Nature += mod.Value; break;

                    //training stat
                    case "Wis": foal.Wis += (int)mod.Value; break;
                    case "Con": foal.Con += (int)mod.Value; break;
            }
        }
    }

    public static List<GeneModel> AncestorGeneExtract(List<(String, int, int)> data)
    {
        List<GeneModel> pedigreeGenes = new();

        foreach((String id, int sGen, int dGen) commonCross in data)
        {
            HorseDataModel ancestor = AncestorsDatabase.Get(commonCross.id);
            string bestGeneId = ancestor.BestGeneId();
            GeneDefinition geneDef = GeneDatabase.Get(bestGeneId);
            if (!pedigreeGenes.Exists(g => g.GeneId == bestGeneId))
            {
                GeneModel bestGene = new GeneModel(bestGeneId, geneDef.BaseGeneStrength);
                pedigreeGenes.Add(bestGene);
            }
        }

        return pedigreeGenes;
    }

    public static void PedigreeEffect(HorseModel foal, PedigreeType type, int level)
    {
        string effectId = EffectIdGenerate(type.ToString(), level);
        switch (type)
        {
            case PedigreeType.OutCross: 
                ApplyPedigreeFlatEffect(foal, "OutCross");
                break;
            case PedigreeType.Inbreeding:
                ApplyPedigreeFlatEffect(foal, "InBreeding");
                break;
            case PedigreeType.LineBreeding:
                ApplyPedigreeFlatEffect(foal, effectId);
                break;
            case PedigreeType.MildInbreeding:
                ApplyPedigreeFlatEffect(foal, effectId);
                break;
        }
    }
}