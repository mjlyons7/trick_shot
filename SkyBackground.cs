using Godot;
using System;

public partial class SkyBackground : ParallaxBackground
{
	CharacterBody2D playerNodeRef;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//// center on player on startup
		//if (playerNodeRef is null)
		//{
		//	playerNodeRef = GetNode<CharacterBody2D>("Player");
		//	Offset = (playerNodeRef is null) ? new Vector2() : playerNodeRef.Position;
		//}
    }
}
