using System.Collections.Generic;
using System.Data;
using System.Linq;
using Horse.Scripts.Models;

namespace Horse.Scripts.Systems;

public static class AncestorsDatabase
{
    private static readonly Dictionary<string, HorseDataModel> _registry = new();
    
    private static void Register(HorseDataModel hData)
    {
        _registry[hData.Id] = hData;
    }

    public static HorseDataModel Get(string id)
    {
        return _registry.TryGetValue(id, out var hData) ? hData : null;
    }

    static AncestorsDatabase()
    {   //base

        //phalaris side
        Register(new HorseDataModel(
            "P-00000000",
            "Phalaris",
            true,
            new() {},
            new() {},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "P-00000002",
            "Pharos",
            true,
            new() {new("P-00000001", 1)},
            new() {},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "P-00000003",
            "Nearco",
            true,
            new() {new("P-00000002", 1), new("P-00000001", 2)},
            new() {},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "P-00000004",
            "Neartic",
            true,
            new() {new("P-00000001", 3), new("P-00000002", 2), new("P-00000003", 1)},
            new() {new("H-00000000", 2), new("H-00000001", 1)},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "P-00000005",
            "Nasrullah",
            true,
            new() {new("P-00000001", 3), new("P-00000002", 2), new("P-00000003", 1)},
            new() {},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "P-00000006",
            "Northen Dancer",
            true,
            new() {new("P-00000001", 4), new("P-00000002", 3), new("P-00000003", 2), new("P-00000004", 1)},
            new() {new("H-00000000", 3), new("H-00000001", 2), new("ND-00000001", 1)},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "P-00000007",
            "Nijinsky",
            true,
            new() {new("P-00000002", 4), new("P-00000003", 3), new("P-00000004", 2), new("P-00000006", 1)},
            new() {new("H-00000000", 4), new("H-00000001", 3), new("ND-00000001", 2)},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "P-00000008",
            "Sadler's Wells",
            true,
            new() {new("P-00000001", 5), new("P-00000002", 4), new("P-00000003", 3), new("P-00000004", 2), new("P-00000006", 1)},
            new() {new("H-00000000", 4), new("H-00000001", 3), new("ND-00000001", 2)},
            new() {new ("Hyper-03")}
        ));

        //Hyperion side
        Register(new HorseDataModel(
            "H-00000000",
            "Hyperion",
            true,
            new() {},
            new() {},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "H-00000001",
            "Lady Angela",
            false,
            new() {new("H-00000000", 1)},
            new() {},
            new() {new ("Hyper-03")}
        ));

        //native dancer side
        Register(new HorseDataModel(
            "ND-00000000",
            "Native Dancer",
            true,
            new() {},
            new() {},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "ND-00000001",
            "Natalma",
            false,
            new() {new("P-00000000", 1)},
            new() {},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "ND-00000002",
            "Raise A Native",
            true,
            new() {new("P-00000000", 1)},
            new() {},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "ND-00000003",
            "Mr Prospector",
            true,
            new() {new("P-00000000", 2), new("P-00000002", 1)},
            new() {},
            new() {new ("Hyper-03")}
        ));

        //bold ruler side
        Register(new HorseDataModel(
            "BR-00000000",
            "Bold Ruler",
            true,
            new() {new("P-00000003", 2), new("P-00000005", 1)},
            new() {},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "BR-00000001",
            "Secretariat",
            true,
            new() {new("P-00000003", 3), new("P-00000005", 2), new("BR-00000000", 1)},
            new() {},
            new() {new ("Hyper-03")}
        ));

        //halo side
        Register(new HorseDataModel(
            "Ha-00000000",
            "Nearco",
            true,
            new() {new("P-00000002", 3), new("P-00000001", 4), new("P-000000003", 2)},
            new() {},
            new() {new ("Hyper-03")}
        ));

        Register(new HorseDataModel(
            "Ha-00000001",
            "Sunday Silence",
            true,
            new() {new("P-00000002", 4), new("P-000000003", 3), new("Ha-00000000", 1)},
            new() {},
            new() {new ("Hyper-03")}
        ));
    }
}