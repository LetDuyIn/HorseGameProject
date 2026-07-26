using Godot;
using System;
using System.Collections.Generic;
using Horse.Scripts.Models;
using Horse.Scripts.Systems;

namespace Horse.Scripts.Tests;

public partial class TestBench : Node
{
    public enum BreedingPreset
    {
        Custom,
        HighVsHigh,     // Thuần chủng x Thuần chủng
        LowVsLow,       // Ngựa rác x Ngựa rác
        HighVsLow       // Phối cải tạo
    }

    [ExportGroup("Test Settings")]
    [Export] public BreedingPreset SelectedPreset = BreedingPreset.Custom;
    [Export] public int SingleTestIterations = 5;
    [Export] public int MassTestIterations = 1000;

    [ExportGroup("Father Config (Bố)")]
    [Export] public string FatherName = "Father_Zeus";
    [Export(PropertyHint.Range, "0.05,0.95,0.01")] public float FatherStability = 0.95f;
    [Export] public Godot.Collections.Array<GeneConfig> FatherGeneConfigs = new();

    [ExportGroup("Mother Config (Mẹ)")]
    [Export] public string MotherName = "Mother_Hera";
    [Export(PropertyHint.Range, "0.05,0.95,0.01")] public float MotherStability = 0.95f;
    [Export] public Godot.Collections.Array<GeneConfig> MotherGeneConfigs = new();

    public override void _Ready()
    {
        GD.Print("==================================================");
        GD.Print("=== BẮT ĐẦU KIỂM THỬ HỆ THỐNG PHỐI GIỐNG (BREEDING) ===");
        GD.Print("==================================================\n");

        ApplyPresetIfSelected();

        var father = BuildHorse(FatherName, FatherStability, FatherGeneConfigs);
        var mother = BuildHorse(MotherName, MotherStability, MotherGeneConfigs);

        RunSingleBreedingTest(father, mother, SingleTestIterations);
        RunMassBreedingTest(father, mother, MassTestIterations);
    }

    private HorseModel BuildHorse(string name, float stability, Godot.Collections.Array<GeneConfig> configs)
    {
        var genes = new List<GeneModel>();
        
        // Nếu không điền gene trong Inspector, fallback về bộ mặc định
        if (configs == null || configs.Count == 0)
        {
            genes.Add(new GeneModel("Spd-05", 0.85f, stability));
            genes.Add(new GeneModel("Hyper-03", 0.70f, stability));
            genes.Add(new GeneModel("Stam-05", 0.65f, stability));
            genes.Add(new GeneModel("Pow-05", 0.50f, stability));
        }
        else
        {
            foreach (var cfg in configs)
            {
                genes.Add(new GeneModel(cfg.GeneId, cfg.Strength, stability));
            }
        }

        return new HorseModel(name, stability, genes);
    }

    private void ApplyPresetIfSelected()
    {
        switch (SelectedPreset)
        {
            case BreedingPreset.HighVsHigh:
                FatherStability = 0.90f;
                MotherStability = 0.85f;
                GD.Print("📌 [PRESET ACTIVATED]: High Stability vs High Stability");
                break;
            case BreedingPreset.LowVsLow:
                FatherStability = 0.15f;
                MotherStability = 0.20f;
                GD.Print("📌 [PRESET ACTIVATED]: Low Stability vs Low Stability");
                break;
            case BreedingPreset.HighVsLow:
                FatherStability = 0.90f;
                MotherStability = 0.20f;
                GD.Print("📌 [PRESET ACTIVATED]: High Stability vs Low Stability");
                break;
            case BreedingPreset.Custom:
            default:
                GD.Print("📌 [PRESET ACTIVATED]: Custom Inspector Settings");
                break;
        }
    }

    private void RunSingleBreedingTest(HorseModel father, HorseModel mother, int iterations)
    {
        GD.Print("\n--- [TEST 1] PHỐI GIỐNG ĐƠN ĐIỂM (SINGLE TEST) ---");

        for (int i = 0; i < iterations; i++)
        {
            var foal = BreedingSystem.Breeding(father, mother, $"Foal_Pegasus_{i + 1}");

            GD.Print($"\n Ngựa con #{i + 1}: {foal.Name} (ID: {foal.Id})");
            GD.Print($"  * Stability Bố ({father.GeneStability:F2}) & Mẹ ({mother.GeneStability:F2}) -> Con: {foal.GeneStability:F3}");
            GD.Print($"  * Tổng số Slot Gene kế thừa: {foal.GenesChain.Count} slots (Tối thiểu: 4)");

            GD.Print("  📊 CHỈ SỐ THỰC TẾ (AFTER APPLY GENETIC):");
            GD.Print($"     - SPEED   : Init = {foal.Spd,4} | Max = {foal.MaxSpd,4}");
            GD.Print($"     - POWER   : Init = {foal.Pow,4} | Max = {foal.MaxPow,4}");
            GD.Print($"     - STAMINA : Init = {foal.Stam,4} | Max = {foal.MaxStam,4}");

            GD.Print("  🧬 Chi tiết Chuỗi Gene (GenesChain):");
            foreach (var g in foal.GenesChain)
            {
                GD.Print($"     + [{g.GeneId,-8}] | Strength: {g.GeneStrength:F3} | Stability: {g.GeneStability:F3}");
            }

            GD.Print("-----------------------------------------------");
        }
    }

    private void RunMassBreedingTest(HorseModel father, HorseModel mother, int iterations)
    {
        GD.Print($"\n--- [TEST 2] CHẠY THỐNG KÊ {iterations:N0} LẦN PHỐI GIỐNG ---");

        int totalBonusGenesObtained = 0;
        int maxGeneCount = 0;
        int minGeneCount = int.MaxValue;
        float totalFoalStability = 0f;

        long sumInitSpd = 0, sumMaxSpd = 0;
        long sumInitPow = 0, sumMaxPow = 0;
        long sumInitStam = 0, sumMaxStam = 0;

        int minInitSpd = int.MaxValue, maxInitSpd = int.MinValue;
        int minMaxSpd = int.MaxValue,  maxMaxSpd = int.MinValue;

        int minInitPow = int.MaxValue, maxInitPow = int.MinValue;
        int minMaxPow = int.MaxValue,  maxMaxPow = int.MinValue;

        int minInitStam = int.MaxValue, maxInitStam = int.MinValue;
        int minMaxStam = int.MaxValue,  maxMaxStam = int.MinValue;

        for (int i = 0; i < iterations; i++)
        {
            var foal = BreedingSystem.Breeding(father, mother, $"Foal_{i}");

            int count = foal.GenesChain.Count;
            if (count > 4) totalBonusGenesObtained += (count - 4);

            maxGeneCount = Math.Max(maxGeneCount, count);
            minGeneCount = Math.Min(minGeneCount, count);
            totalFoalStability += foal.GeneStability;

            sumInitSpd += foal.Spd; sumMaxSpd += foal.MaxSpd;
            minInitSpd = Math.Min(minInitSpd, foal.Spd); maxInitSpd = Math.Max(maxInitSpd, foal.Spd);
            minMaxSpd = Math.Min(minMaxSpd, foal.MaxSpd); maxMaxSpd = Math.Max(maxMaxSpd, foal.MaxSpd);

            sumInitPow += foal.Pow; sumMaxPow += foal.MaxPow;
            minInitPow = Math.Min(minInitPow, foal.Pow); maxInitPow = Math.Max(maxInitPow, foal.Pow);
            minMaxPow = Math.Min(minMaxPow, foal.MaxPow); maxMaxPow = Math.Max(maxMaxPow, foal.MaxPow);

            sumInitStam += foal.Stam; sumMaxStam += foal.MaxStam;
            minInitStam = Math.Min(minInitStam, foal.Stam); maxInitStam = Math.Max(maxInitStam, foal.Stam);
            minMaxStam = Math.Min(minMaxStam, foal.MaxStam); maxMaxStam = Math.Max(maxMaxStam, foal.MaxStam);
        }

        GD.Print($"\n==================================================");
        GD.Print($"📌 KẾT QUẢ THỐNG KÊ SAU {iterations:N0} LẦN PHỐI GIỐNG");
        GD.Print($"==================================================");
        
        GD.Print("\n🧬 THỐNG KÊ GENE & STABILITY:");
        GD.Print($"* Bonus Gene trung bình bốc được : {(float)totalBonusGenesObtained / iterations:F2} Gene/con");
        GD.Print($"* Chuỗi Gene ngắn nhất / dài nhất: {minGeneCount} slots / {maxGeneCount} slots");
        GD.Print($"* Stability trung bình đàn con    : {totalFoalStability / iterations:F3}");

        GD.Print("\n📈 THỐNG KÊ CHỈ SỐ (STATS) CỦA ĐÀN CON:");
        GD.Print($"------------------------------------------------------------------");
        GD.Print($"{"CHỈ SỐ",-10} | {"TRUNG BÌNH (AVG)",-20} | {"THẤP NHẤT (MIN)",-18} | {"CAO NHẤT (MAX)",-18}");
        GD.Print($"------------------------------------------------------------------");
        
        GD.Print($"{"Init Spd",-10} | {(float)sumInitSpd / iterations,20:F1} | {minInitSpd,18} | {maxInitSpd,18}");
        GD.Print($"{"Max Spd",-10} | {(float)sumMaxSpd / iterations,20:F1} | {minMaxSpd,18} | {maxMaxSpd,18}");
        GD.Print($"------------------------------------------------------------------");
        
        GD.Print($"{"Init Pow",-10} | {(float)sumInitPow / iterations,20:F1} | {minInitPow,18} | {maxInitPow,18}");
        GD.Print($"{"Max Pow",-10} | {(float)sumMaxPow / iterations,20:F1} | {minMaxPow,18} | {maxMaxPow,18}");
        GD.Print($"------------------------------------------------------------------");

        GD.Print($"{"Init Stam",-10} | {(float)sumInitStam / iterations,20:F1} | {minInitStam,18} | {maxInitStam,18}");
        GD.Print($"{"Max Stam",-10} | {(float)sumMaxStam / iterations,20:F1} | {minMaxStam,18} | {maxMaxStam,18}");
        GD.Print($"------------------------------------------------------------------\n");
    }
}

// Struct phụ trợ giúp tạo List Gene dễ dàng trong Godot Inspector
[GlobalClass]
public partial class GeneConfig : Resource
{
    [Export] public string GeneId { get; set; } = "Spd-01";
    [Export(PropertyHint.Range, "0.01,1.0,0.01")] public float Strength { get; set; } = 0.5f;

    public GeneConfig() { }

    public GeneConfig(string geneId, float strength)
    {
        GeneId = geneId;
        Strength = strength;
    }
}