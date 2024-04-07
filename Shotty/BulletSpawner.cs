using Godot;
using System;
using System.Diagnostics;

public partial class BulletSpawner : Node2D
{
    [Export]
    PackedScene bulletScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("shoot"))
            Shoot(Transform);
    }

    protected void Shoot(Transform2D bulletGlobalTransform)
    {
        var bullet = bulletScene.Instantiate<Bullet>();
        bullet.Transform = bulletGlobalTransform;
        GetNode("/root").AddChild(bullet);
    }
}
