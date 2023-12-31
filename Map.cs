using Godot;
using System;
using System.Diagnostics;

public partial class Map : Node2D
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
        {
            var shottyRef = GetNode<Shotty>(Shotty.ClassName);
            if (shottyRef is not null)
            {
                var bullet = bulletScene.Instantiate<Bullet>();
                // bullet.Position = shottyRef.Position;
                // bullet.Rotation = shottyRef.Rotation;
                bullet.Transform = shottyRef.Transform;
                AddChild(bullet);
            }
            else
                Debug.WriteLine("Reference to shotty not found");
        }
    }
}
