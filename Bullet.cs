using Godot;
using System;
using System.Diagnostics;

public partial class Bullet : Area2D
{
	float speed = 1000; // pixels per second
	Vector2 direction;
	DebugHelper helper;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));

		helper = new DebugHelper();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += ((float)delta * speed) * direction;

		helper.Run(delta);
	}

	private void OnAreaEntered(Area2D area)
	{
		Rotation = (float)(Math.PI - Rotation) + (2 * area.Rotation);
		direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
		return;
	}
}
