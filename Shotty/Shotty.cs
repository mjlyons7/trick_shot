using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

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

        // vertically center sprite around x-axis
        Offset = new Vector2(0, -(Texture.GetSize().Y / 2));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        helper.Run(delta);
        var debugStrings = new List<string>();

        // rotation
        if (player is not null)
            Position = player.Position;
        else
        {
            debugStrings.Add("WARNING: player not found");
        }
        rotationOrigin = GlobalPosition;
        RotateToTarget();

        // animation
        if ((Rotation < -Math.PI / 2) || (Rotation > Math.PI / 2))
        {
            FlipV = true;
        }
        else
        {
            FlipV = false;
        }

        // print debug info
        HelperPrint(debugStrings, delta);
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
        var target = GetGlobalMousePosition();
        return target;
    }

    private void HelperPrint(List<string> debugStrings, double delta)
    {
        // var rotationDegrees = Rotation * 180 / Math.PI;
        // strings.Add("Rotation: " + rotationDegrees.ToString());
        debugStrings.Add("--------------");
        debugStrings.Add("Mouse pos: " + GetTargetPoint().ToString());
        debugStrings.Add("rot source: " + rotationOrigin.ToString());
        debugStrings.Add("target vector: " + targetVector.ToString());
        debugStrings.Add("rotation: " + RotationDegrees.ToString());

        helper.PrintAfterInterval(debugStrings);
    }
}
