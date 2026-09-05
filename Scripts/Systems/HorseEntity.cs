using Godot;
using System;

namespace Horse.Scripts.Models;

public partial class HorseEnity : Node2D
{
	public HorseModel Data{get; private set;}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
