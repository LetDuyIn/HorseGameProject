namespace Horse.Scripts.Models;

public class GeneModel
{
    public string GeneId {get; set;}
    public float GeneStrength {get; set;}
    public float GeneStability{get; set;}

    public GeneModel(){}

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
}