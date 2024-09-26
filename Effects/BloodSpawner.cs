using Godot;
using System;

public partial class BloodSpawner : Node2D
{
    [Export]
    PackedScene bloodDropScene;

    double bloodSpurtTime = 1.5; // how many seconds to shoot blood
    double bloodSpurtRate = 20; // blood drops per second

    float offsetDegrees; // shift blood drops a little when spawning
    double runtime; // time it's been active
    double timeSinceLastDrop;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // just give it a high value to start, so will shoot blood on first frame
        timeSinceLastDrop = 999;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        runtime += delta;
        timeSinceLastDrop += delta;

        // despawn self - stop the bleeding!
        if (runtime > bloodSpurtTime)
            QueueFree();

        // raining blood!
        if (timeSinceLastDrop > (1 / bloodSpurtRate))
        {
            timeSinceLastDrop = 0;
            InstantiateBlooddrop();
        }
    }

    private void InstantiateBlooddrop()
    {
        // todo: need to rotate to blood drop's direction, based on the transform
        float forceMagnitude = 500;
        Vector2 impulseForce = new Vector2(1, 0);
        impulseForce *= forceMagnitude;

        // rotate transform and force
        var bloodDrop = bloodDropScene.Instantiate<BloodDrop>();
        bloodDrop.Position = GlobalPosition;
        bloodDrop.Transform = GlobalTransform;
        bloodDrop.RotationDegrees += offsetDegrees;
        impulseForce = impulseForce.Rotated(bloodDrop.RotationDegrees * (float)(Math.PI/180));

        // add some random variation
        var randGen = new Random();
        offsetDegrees = (float)((randGen.NextDouble() * 40) - 20);

        // apply force in opposite direction (the direction the hit came from)
        GetNode("/root").AddChild(bloodDrop);
        bloodDrop.ApplyImpulse(impulseForce, bloodDrop.Position);
    }
}
