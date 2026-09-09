namespace Horse.Scripts.Models;

public class Ancestor
{
    public string Id {get; set;}
    public string Name {get; set;}
    public int Generation {get; set;}

    public Ancestor(string id, int gen)
    {
        Id = id;
        Generation = gen;
    }
    
}