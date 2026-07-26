using System;
using System.Collections.Generic;
using Godot;
using Horse.Scripts.Models;
using Horse.Scripts.Systems;

namespace Horse.Scripts.Tests;

public partial class TestBreedAndGrow : Node
{
    public override void _Ready()
    {
        RunBreedingTest();
    }

    private void RunBreedingTest()
    {
        GD.Print("==================================================");
        GD.Print("        BẮT ĐẦU TEST HỆ THỐNG PHỐI GIỐNG & TĂNG TRƯỞNG");
        GD.Print("==================================================\n");

        // 1. Tạo Dữ Liệu Giả Cho Bố (Sire)
        var fatherGenes = new List<GeneModel>
        {
            new GeneModel("Spd-03", 0.8f, 0.85f),
            new GeneModel("Stam-03", 0.6f, 0.85f),
            new GeneModel("Pow-03", 0.7f, 0.85f),
            new GeneModel("Spd-05", 0.1f, 0.85f)
        };
        var father = new HorseModel("Sire Alpha", 0.85f, fatherGenes)
        {
            Nature = 80,
            GrowthRate = 2.2f,
            DecayRate = 1.1f
        };
        father.ApplyGenetic();

        // 2. Tạo Dữ Liệu Giả Cho Mẹ (Dam)
        var motherGenes = new List<GeneModel>
        {
            new GeneModel("Pow-05", 0.75f, 0.80f),
            new GeneModel("Stam-02", 0.65f, 0.80f),
            new GeneModel("Spd-05", 0.5f, 0.80f),
            new GeneModel("Stam-02", 0.5f, 0.80f)
        };
        var mother = new HorseModel("Dam Beta", 0.80f, motherGenes)
        {
            Nature = 70,
            GrowthRate = 1.8f,
            DecayRate = 0.9f
        };
        mother.ApplyGenetic();

        // In thông tin bố mẹ
        PrintHorseProfile("BỐ (SIRE)", father);
        PrintHorseProfile("MẸ (DAM)", mother);

        // 3. Tiến Hành Phối Giống
        GD.Print("--------------------------------------------------");
        GD.Print(">>> ĐANG PHỐI GIỐNG...");
        GD.Print("--------------------------------------------------");
        
        HorseModel foal = BreedingSystem.Breeding(father, mother, "Foal Omega");

        // In thông tin ngựa con vừa sinh
        PrintHorseProfile("NGỰA CON (FOAL) LÚC MỚI SINH", foal);

        // 4. Mô Phỏng Quá Trình Tăng Trưởng Theo Tuần (Grow Loop)
        GD.Print("--------------------------------------------------");
        GD.Print($">>> BẮT ĐẦU QUÁ TRÌNH TĂNG TRƯỞNG (ĐỈNH ĐIỂM Ở TUẦN {foal.Peak})");
        GD.Print("--------------------------------------------------");
        GD.Print("Tuần\t| Tuổi\t| Spd\t| Pow\t| Stam\t| Giai Đoạn");
        GD.Print("--------+-------+-------+-------+-------+------------------");

        int totalWeeksToSimulate = foal.Peak + 15; // Giả lập qua cả đỉnh điểm để xem suy thoái

        for (int week = 0; week <= totalWeeksToSimulate; week++)
        {
            string phase = foal.Age <= foal.Peak ? "Phát triển" : "Suy thoái";
            
            // In thông tin mỗi 5 tuần hoặc vào đúng tuần Đỉnh (Peak)
            if (week % 5 == 0 || foal.Age == foal.Peak)
            {
                GD.Print($"{week}\t| {foal.Age}\t| {foal.Spd}\t| {foal.Pow}\t| {foal.Stam}\t| {phase}{(foal.Age == foal.Peak ? " [ĐỈNH PEAK]" : "")}");
            }

            // Gọi hàm Grow() để ngựa lớn lên
            foal.Grow();
        }

        GD.Print("\n==================================================");
        GD.Print("               HOÀN THÀNH KỊCH BẢN TEST");
        GD.Print("==================================================");
    }

    private void PrintHorseProfile(string header, HorseModel horse)
    {
        GD.Print($"\n--- [{header}: {horse.Name}] ---");
        GD.Print($"  + Gene Stability : {horse.GeneStability:F2}");
        GD.Print($"  + Nature         : {horse.Nature}");
        GD.Print($"  + Growth / Decay : {horse.GrowthRate:F2} / {horse.DecayRate:F2}");
        GD.Print($"  + Peak Age       : Tuần {horse.Peak}");
        GD.Print($"  + Stats Hiện Tại : Spd {horse.Spd}/{horse.MaxSpd} | Pow {horse.Pow}/{horse.MaxPow} | Stam {horse.Stam}/{horse.MaxStam}");
        GD.Print($"  + Chuỗi Gen ({horse.GenesChain.Count} gen): " + 
                 string.Join(", ", horse.GenesChain.ConvertAll(g => $"{g.GeneId}(str:{g.GeneStrength:F2})")));
    }
}