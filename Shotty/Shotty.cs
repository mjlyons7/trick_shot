using Godot;
using System;
using System.Collections.Generic;

public partial class Shotty : Sprite2D
{
    Vector2 rotationOrigin;
    DebugHelper helper;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        rotationOrigin = Position;
        rotationOrigin.Y += Scale.Y / 2;
        helper = new DebugHelper();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        rotateToTarget();

        // debug strings
        helper.Run(delta);
        var strings = new List<string>();
        var rotationDegrees = Rotation * 180 / Math.PI;
        strings.Add("Rotation: " + rotationDegrees.ToString());
        helper.PrintAfterInterval(strings);
    }

    // rotate towards the target
    private void rotateToTarget()
    {
        var targetVector = GetTargetPoint() - rotationOrigin;
        Rotation = Mathf.Atan2(targetVector.Y, targetVector.X);
    }

    private Vector2 GetTargetPoint()
    {
        // target the mouse
        var target = GetViewport().GetMousePosition();
        return target;
    }
}
