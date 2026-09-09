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

    public RanchDataModel ToSaveData()
    {
        RanchDataModel data = new();

        data.Id = this.Id;
        data.Name = this.Name;
        data.OwnerName =this.OwnerName;

        foreach(var h in this.HorsesList)
        {
            data.HorsesDataList.Add(h.ToSaveData());
        }

        return data;
    }

    public void FromSavedData(RanchDataModel data)
    {
        this.HorsesList.Clear();

        this.Id = data.Id;
        this.Name = data.Name;
        this.OwnerName = data.OwnerName;

        foreach(var d in data.HorsesDataList)
        {
            HorseModel h = new HorseModel();
            h.FromSavedData(d);
            this.HorsesList.Add(h);
        }
    }
}