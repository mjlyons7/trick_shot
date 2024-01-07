using Godot;
using System;
using System.Diagnostics;

public partial class Bullet : Area2D
{
	float speed = 1000; // pixels per second
	Vector2 direction;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += ((float)delta * speed) * direction;
	}

	private void OnBodyEntered(Node2D body)
	{
		Debug.WriteLine("Body entered: " + body.ToString());
	}
}
