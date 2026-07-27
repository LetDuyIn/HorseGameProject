using System;
using System.Collections.Generic;
using Godot;
using Horse.Scripts.Models;
using Horse.Scripts.Systems;

namespace Horse.Scripts.Tests;

public partial class GrowthCompare : Node
{
    public override void _Ready()
    {
        RunTest();
    }

    public static void RunTest()
    {
        GD.Print("=== BẮT ĐẦU SIMULATION TĂNG TRƯỞNG NGỰA ===");

        // -------------------------------------------------------------
        // 1. TẠO CẶP BỐ MẸ A (Dòng giống Chín Sớm)
        // -------------------------------------------------------------
        var geneChain = new List<GeneModel> 
        { 
            new GeneModel("Stam-03", 0.8f, 0.85f),
            new GeneModel("Stam-03", 0.6f, 0.85f),
            new GeneModel("Pow-03", 0.7f, 0.85f),
            new GeneModel("Spd-05", 0.1f, 0.85f) 
        };
        var fatherA = new HorseModel("Father_Early", 0.8f, geneChain) 
        { 
            Nature = 3, GrowthRate = 0.8f, DecayRate = 1.4f, PeakFactor = 1.0f 
        };
        var motherA = new HorseModel("Mother_Early", 0.8f, geneChain) 
        { 
            Nature = 3, GrowthRate = 0.8f, DecayRate = 1.4f, PeakFactor = 1.0f
        };

        // -------------------------------------------------------------
        // 2. TẠO CẶP BỐ MẸ B (Dòng giống Chín Muộn)
        // -------------------------------------------------------------
        
        var fatherB = new HorseModel("Father_Late", 0.8f,geneChain) 
        { 
            Nature = 3, GrowthRate = 3.0f, DecayRate = 0.8f, PeakFactor = 1.4f 
        };
        var motherB = new HorseModel("Mother_Late", 0.8f, geneChain) 
        { 
            Nature = 3, GrowthRate = 3.0f, DecayRate = 0.8f, PeakFactor = 1.8f
        };

        // -------------------------------------------------------------
        // 3. THỰC HIỆN LAI TẠO ĐỂ TẠO 2 CON
        // -------------------------------------------------------------
        HorseModel foalA = BreedingSystem.Breeding(fatherA, motherA, "Foal_Early_A");
        HorseModel foalB = BreedingSystem.Breeding(fatherB, motherB, "Foal_Late_B");

        GD.Print($"\n[KHỞI TẠO SUỐT] ");
        GD.Print($"-> {foalA.Name}: Peak Target = {foalA.Peak} tuần | Nat = {foalA.Nature} | GRate = {foalA.GrowthRate:F2} | DRate = {foalA.DecayRate} | PFactor = {foalA.PeakFactor}");
        GD.Print($"-> {foalB.Name}: Peak Target = {foalB.Peak} tuần | Nat = {foalB.Nature} | GRate = {foalB.GrowthRate:F2} | DRate = {foalB.DecayRate} | PFactor = {foalB.PeakFactor}");
        GD.Print("-------------------------------------------------------------");

        // -------------------------------------------------------------
        // 4. VÒNG LẶP GROW & LƯU THÔNG TIN PEAK
        // -------------------------------------------------------------
        bool recordedA = false; bool recordedZeroA = false;
        bool recordedB = false; bool recordedZeroB = false;

        // Biến lưu thông tin khi đạt Peak
        int peakAgeA = 0, peakSpdA = 0, maxSpdA = 0, zeroAgeA = 0;
        int peakAgeB = 0, peakSpdB = 0, maxSpdB = 0, zeroAgeB = 0;

        int totalSimWeeks = 700; // Mô phỏng 300 tuần (~5.7 năm)

        for (int week = 0; week <= totalSimWeeks; week++)
        {
            // Kiểm tra và ghi nhận khi Foal A đạt Peak
            if (!recordedA && foalA.Age == foalA.Peak)
            {
                peakAgeA = foalA.Age;
                peakSpdA = foalA.Spd;
                maxSpdA = foalA.MaxSpd;
                recordedA = true;
            }

            // Kiểm tra và ghi nhận khi Foal B đạt Peak
            if (!recordedB && foalB.Age == foalB.Peak)
            {
                peakAgeB = foalB.Age;
                peakSpdB = foalB.Spd;
                maxSpdB = foalB.MaxSpd;
                recordedB = true;
            }

            if (!recordedZeroA && (foalA.Spd + foalA.Stam + foalA.Pow) == 0 && foalA.Age > foalA.Peak)
            {
                zeroAgeA = foalA.Age;
                recordedZeroA = true;
            }

            if (!recordedZeroB && (foalB.Spd + foalB.Stam + foalB.Pow) == 0 && foalB.Age > foalB.Peak)
            {
                zeroAgeB = foalB.Age;
                recordedZeroB = true;
            }

            // Cho cả 2 con cùng Grow 1 tuần
            foalA.Grow();
            foalB.Grow();
        }

        // -------------------------------------------------------------
        // 5. IN KẾT QUẢ SO SÁNH
        // -------------------------------------------------------------
        GD.Print("\n================ KẾT QUẢ SO SÁNH PHONG ĐỘ ================");
        
        PrintHorsePeakReport(foalA.Name, peakAgeA, peakSpdA, maxSpdA, zeroAgeA);
        PrintHorsePeakReport(foalB.Name, peakAgeB, peakSpdB, maxSpdB, zeroAgeB);

        GD.Print("==========================================================");
    }

    private static void PrintHorsePeakReport(string name, int peakAge, int currentStat, int maxStat, int zeroAge)
    {
        float ratio = ((float)currentStat / maxStat) * 100f;
        GD.Print($"\n🐎 [NGỰA: {name}]");
        GD.Print($"  • Thời điểm đạt Peak : Tuần thứ {peakAge} (~{peakAge / 48.0f:F1} năm)");
        GD.Print($"  • Chỉ số Speed tại Peak: {currentStat} / {maxStat}");
        GD.Print($"  • Tỷ lệ đạt MaxStat   : {ratio:F2}%");
        GD.Print($"  • Thoi diem zero stat: Tuần thứ {zeroAge}");
    }
}