using Horse.Scripts.Systems;

namespace Horse.Scripts.Models;

public class GeneModel
{
    public string GeneId {get; set;}
    public float GeneStrength {get; set;}
    public float GeneStability{get; set;}

    public GeneModel(){}

    public GeneModel(string id)
    {
        GeneId = id;
    }

    public GeneModel(string id, float str)
    {
        GeneId = id;
        GeneStrength = str;
    }
    public GeneModel(string id, float str, float stability)
    {
        GeneId = id;
        GeneStrength = str;
        GeneStability = stability;
    }

    public float GeneEvaluate()
    {
        float totalStatBonus = 0;
        float str = this.GeneStrength;
        GeneDefinition gene = GeneDatabase.Get(this.GeneId);

        foreach(var mod in gene.StatModifiers)
        {
            totalStatBonus += mod.Value;
        }

        return totalStatBonus * str;
    }
}