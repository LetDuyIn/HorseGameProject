using Godot;
using System;
using System.Text.Json;
using Horse.Scripts.Models;
using System.Collections.Generic;

namespace Horse.Scripts.Systems;

public static class DataManager
{
    private static readonly String PlayerRanchPath = "user://player_ranch.json";
    private static readonly String NPCRanchesPath = "user://npc_ranches.json";

    public static RanchModel PlayerRanch {get; set;}
    public static List<RanchModel> NPCRanchesList {get; set;} = new();

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    //save
    public static void SavePlayerRanch()
    {
        if(PlayerRanch == null) return;
        try
        {
            string jsonString = JsonSerializer.Serialize(PlayerRanch.ToSaveData(), JsonOptions);
            WriteFile(PlayerRanchPath, jsonString);
            GD.Print("[DataManager] save player successfully");
        }catch(Exception e)
        {
            GD.PrintErr($"[DataManager] save player error: {e.Message}");
        }
    }

    public static void SaveNPCRanches()
    {
        try
        {
            List<RanchDataModel> dataList = new();
            foreach(var npcR in NPCRanchesList)
            {
                dataList.Add(npcR.ToSaveData());
            }
            string jsonString = JsonSerializer.Serialize(dataList, JsonOptions);
            WriteFile(NPCRanchesPath, jsonString);
            GD.Print("[DataManager] save NPC successfully");
        }catch(Exception e)
        {
            GD.PrintErr($"[DataManager] save NPC error: {e.Message}");
        }
    }

    public static void SaveAll()
    {
        SavePlayerRanch();
        SaveNPCRanches();
    }

    //load
    public static void LoadPlayerRanch()
    {
        if (!FileAccess.FileExists(PlayerRanchPath))
        {
            GD.Print("[DataManager] Cant find the player file. Initialize first time play player");
            PlayerRanch = new RanchModel("PlayerRanch", "Player");
            //PlayerRanch.HorsesList.Add(new HorseModel("Beginner Horse"));
            return;
        }

        try
        {
            string json = ReadFile(PlayerRanchPath);
            PlayerRanch = new RanchModel();
            PlayerRanch.FromSavedData(JsonSerializer.Deserialize<RanchDataModel>(json));
            GD.Print($"[DataManager] Player ranch loaded: {PlayerRanch.Name}");
        }catch(Exception e)
        {
            GD.PrintErr($"[DataManager] load player ranch error: {e.Message}");
        }
    }

    public static void LoadNPCRanches()
    {
        if (!FileAccess.FileExists(NPCRanchesPath))
        {
            GD.Print("[DataManager] Cant find the NPC file. Initialize first time play NPC");
            InitializeNPC();
            return;
        }

        try
        {
            NPCRanchesList.Clear();
            string json = ReadFile(NPCRanchesPath);
            List<RanchDataModel> dataSet = JsonSerializer.Deserialize<List<RanchDataModel>>(json);
            foreach(RanchDataModel d in dataSet)
            {
                RanchModel npcR = new RanchModel();
                npcR.FromSavedData(d);
                NPCRanchesList.Add(npcR);
            }
            
            GD.Print($"[DataManager] NPC ranches loaded: {NPCRanchesList.Count} ranches");
        }catch(Exception e)
        {
            GD.PrintErr($"[DataManager] load NPC ranches error: {e.Message}");
        }
    }

    public static void LoadAll()
    {
        LoadPlayerRanch();
        LoadNPCRanches();
    }

    private static void InitializeNPC()
    {
        NPCRanchesList.Clear();

        RanchModel npc1 = new RanchModel("NPC1 Ranch", "Rival_1");
        //npc1.HorsesList.Add(new HorseModel("NPC1 Horse"));

        RanchModel npc2 = new RanchModel("NPC2 Ranch", "Rival_2");
        //npc2.HorsesList.Add(new HorseModel("NPC2 Horse"));

        NPCRanchesList.Add(npc1);
        NPCRanchesList.Add(npc2);
    }

    private static void WriteFile(string path, string content)
    {
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        file?.StoreString(content);
    }

    private static string ReadFile(string path)
    {
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        return file?.GetAsText() ?? string.Empty;
    }
}