using System;
using System.Collections.Generic;

namespace  Horse.Scripts.Models;

public class RanchModel
{
    public String Id {get; private set;} = Guid.NewGuid().ToString().Substring(0, 8);

    public String Name{get; set;}

    public String OwnerName{get; set;}

    public List<HorseModel> HorsesList{get; set;} = new();

    public RanchModel() { }

    public RanchModel(String name, String ownerName)
    {
        Name = name;
        OwnerName = ownerName;
    }
}