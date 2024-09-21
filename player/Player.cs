using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;

public partial class Player : CharacterBody2D, IMortal
{
    [Export]
    PackedScene BloodSpawnerScene;

    int _hitPoints = 1;
    public int HitPoints
    {
        get => _hitPoints;
        set => _hitPoints = value;
    }

    bool status_inAir;

    // internal use
    double baseSpeed = 6e2; // pixels per second
    double terminalVelocity = 5e3;
    double initialJumpVelocity = 1e3;
    double projectGravity;
    double gravityMultiplier = 2e0;

    Vector2 prevVelocity;
    PlayerState prevPlayerState;

    AnimatedSprite2D playerSprite;
    DebugHelper helper;

    PlayerState playerState;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        projectGravity = (double) ProjectSettings.GetSetting("physics/2d/default_gravity");
        playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        playerSprite.Animation = "walk";

        
        playerState = PlayerState.IDLE;

        helper = new DebugHelper();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // todo: figure out a better logging system
        helper.Run(delta);

        if (Globals.PAUSE_ON)
            Pause();
        else if (playerState == PlayerState.PAUSED)
            Unpause();

        switch (playerState)
        {
            case PlayerState.IDLE:
                RunAndJump(delta);
                break;
            case PlayerState.DEAD:
                // TODO: ?? some kind of death animation
                break;
        }

        // debug print
        if (helper.intervalElapsed)
        {
            var strings = new List<string>();
            //strings.Add("direction input: " + directionVector.ToString());
            strings.Add("Velocity: " + Velocity.ToString());
            helper.PrintAfterInterval(strings);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();

        // TODO: check if on the ground with ray tracing
    }

    private void Pause()
    {
        if (playerState == PlayerState.PAUSED)
            return;

        prevVelocity = Velocity;
        Velocity = new Vector2(0, 0);

        prevPlayerState = playerState;
        playerState = PlayerState.PAUSED;

        playerSprite.Pause();
    }

    private void Unpause()
    {
        Debug.Assert(playerState == PlayerState.PAUSED, "Unpause called, but player is already unpaused");

        Velocity = prevVelocity;
        playerState = prevPlayerState;
    }

    private void RunAndJump(double delta)
    {
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
        // 60 pixels/sec 
        var velocity = Velocity;
        velocity.X = directionVector.X * (float)baseSpeed;

        // v = v0 + at
        velocity.Y += (float)(gravityMultiplier * projectGravity * delta);
        if (jumpPressedThisFrame && (!status_inAir))
            velocity.Y = (float)-initialJumpVelocity;
        else if (velocity.Y > terminalVelocity)
            velocity.Y = (float)terminalVelocity;
        Velocity = velocity;

        // animation
        if (Velocity.X < 0)
            playerSprite.FlipH = true;
        else if (Velocity.X > 0)
            playerSprite.FlipH = false;

        if (Velocity.X != 0)
            playerSprite.Play();
        else
            playerSprite.Stop();
    }

    #region IMortal Overrides
    public void OnHit(Vector2 hitPosition, Vector2 hitDirection)
    {
        HitPoints--;
        if ((HitPoints < 1) && (playerState != PlayerState.DEAD))
        {
            Die();
        }

        if (playerState == PlayerState.DEAD)
        {
            var bloodSpawner = BloodSpawnerScene.Instantiate<BloodSpawner>();
            bloodSpawner.Position = hitPosition - GlobalPosition;
            var hitAngle = (float)Math.Atan2((double)hitDirection[1], (double)hitDirection[0]);
            bloodSpawner.Rotate(hitAngle + (float)Math.PI);
            AddChild(bloodSpawner);
        }
    }

    public void Die()
    {
        playerState = PlayerState.DEAD;
        Velocity = new Vector2(0, 0);
    }
    #endregion
}
