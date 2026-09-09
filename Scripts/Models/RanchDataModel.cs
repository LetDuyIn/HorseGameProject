using System;
using System.Collections.Generic;

namespace  Horse.Scripts.Models;

public class RanchDataModel
{
    public String Id {get; set;} = Guid.NewGuid().ToString().Substring(0, 8);

    public String Name{get; set;}

    public String OwnerName{get; set;}

    public List<HorseDataModel> HorsesDataList{get; set;} = new();

    public RanchDataModel(){}

    public RanchDataModel(String name, String ownerName)
    {
        Name = name;
        OwnerName = ownerName;
    }
}