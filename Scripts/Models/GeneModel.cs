namespace Horse.Scripts.Models;

public class GeneModel
{
    public string GeneId {get; set;}
    public float GeneStrength {get; set;}

    public GeneModel(){}

    public GeneModel(string id, float str)
    {
        GeneId = id;
        GeneStrength = str;
    }
}