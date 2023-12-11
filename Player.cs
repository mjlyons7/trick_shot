using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody2D
{
    double timePassed;
    long framesSinceLastSecond;
    double timeSinceLastSecond;
    double fps;
    double baseSpeed = 5*10e3; // pixels per second
    double terminalVelocity = 5*10e2;
    double initialJumpVelocity = 1 * 10e2;
    double projectGravity;
    double gravityMultiplier = 5 * 10e1;

    string walkAnimationName = "walk";
    AnimatedSprite2D playerSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        projectGravity = (double) ProjectSettings.GetSetting("physics/2d/default_gravity");
        playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        playerSprite.Animation = walkAnimationName;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // tracking time
        timePassed += delta;
        timeSinceLastSecond += delta;
        framesSinceLastSecond += 1;
        bool secondPassedThisFrame = false;
        if (timeSinceLastSecond > 1)
        {
            fps = framesSinceLastSecond / timeSinceLastSecond;
            timeSinceLastSecond = 0;
            framesSinceLastSecond = 0;
            secondPassedThisFrame = true;
        }
        

        // get inputs
        var directionVector = new Vector2();

        if (Input.IsActionPressed("ui_right"))
            directionVector.X += 1;
        if (Input.IsActionPressed("ui_left"))
            directionVector.X += -1;
        if (Input.IsActionPressed("ui_up"))
            directionVector.Y += -1;
        if (Input.IsActionPressed("ui_down"))
            directionVector.Y += 1;

        var jumpPressedThisFrame = (Input.IsActionJustPressed("jump"));

        // apply motion
        // directionVector.X * baseSpeed * delta
        var velocity = Velocity;
        velocity.X = directionVector.X * (float)baseSpeed * (float)delta;

        velocity.Y += (float) ((0.5) * gravityMultiplier * projectGravity * Math.Pow(delta, 2));
        if (jumpPressedThisFrame)
            velocity.Y = (float) -initialJumpVelocity;
        if (velocity.Y > terminalVelocity)
            velocity.Y = (float)terminalVelocity;
        Velocity = velocity;
        MoveAndSlide();

        // animation
        if (Velocity.X < 0)
            playerSprite.FlipH = true;
        else if (Velocity.X > 0)
            playerSprite.FlipH = false;

        if (Velocity.X != 0)
            playerSprite.Play();
        else
            playerSprite.Stop();

        // TODO: erase later, just for debugging
        if (secondPassedThisFrame)
        {
            Debug.WriteLine("direction input: "+directionVector.ToString());
            Debug.WriteLine("Velocity: "+Velocity.ToString());
            Debug.WriteLine("FPS: " + fps.ToString());
        }
    }
}
