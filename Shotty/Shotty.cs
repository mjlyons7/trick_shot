using Godot;
using System;
using System.Collections.Generic;

public partial class Shotty : Sprite2D
{
    Vector2 rotationOrigin;
    Vector2 targetVector;
    CharacterBody2D player;

    DebugHelper helper;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        helper = new DebugHelper();
        player = GetParent().GetNode<CharacterBody2D>("Player");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        helper.Run(delta);

        Position = player.Position;
        rotationOrigin = GlobalPosition;
        RotateToTarget();

        HelperPrint(delta);
    }

    // rotate towards the target
    private void RotateToTarget()
    {
        targetVector = GetTargetPoint() - rotationOrigin;
        var targetRotation = Mathf.Atan2(targetVector.Y, targetVector.X);
        Rotation = targetRotation;
    }

    private Vector2 GetTargetPoint()
    {
        // return mouse's position in game world coordinates
        // TODO: get mouse position is out of date after physics calc...
        var target = GetGlobalMousePosition();
        return target;
    }

    private void HelperPrint(double delta)
    {
        // debug strings
        var strings = new List<string>();

        // var rotationDegrees = Rotation * 180 / Math.PI;
        // strings.Add("Rotation: " + rotationDegrees.ToString());
        strings.Add("--------------");
        strings.Add("Mouse pos: " + GetTargetPoint().ToString());
        strings.Add("rot source: " + rotationOrigin.ToString());
        strings.Add("target vector: " + targetVector.ToString());

        helper.PrintAfterInterval(strings);
    }
}
