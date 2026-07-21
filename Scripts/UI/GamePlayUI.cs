using Godot;
using System;
using Horse.Scripts.System; 
using Horse.Scripts.Models;

namespace Horse.Scripts.UI;

public partial class GamePlayUI : Control
{
    private VBoxContainer _horseListContainer;
    private Button _btnLoadData;

    public override void _Ready()
    {
        _horseListContainer = GetNode<VBoxContainer>("HorseListContainner");
        _btnLoadData = GetNode<Button>("BtnLoad");

        _btnLoadData.Pressed += OnLoadDataPressed;

        DataManager.LoadAll();
    }

    private void OnLoadDataPressed()
    {
        foreach (Node child in _horseListContainer.GetChildren())
        {
            child.QueueFree(); 
        }

        if (DataManager.PlayerRanch != null && DataManager.PlayerRanch.HorsesList != null)
        {
            foreach (HorseModel horse in DataManager.PlayerRanch.HorsesList)
            {
                Label horseLabel = new Label();
                
                horseLabel.Text = $"Horse ID: {horse.Id} - Tên: {horse.Name}";
                
                _horseListContainer.AddChild(horseLabel);

                GD.Print($"[UI Test] Label add: {horse.Name}");
            }
        }
    }
}