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
    Vector2 deathHitDirection;

    double timeDead;
    bool fallenOver;

    AnimatedSprite2D playerSprite;
    CollisionShape2D playerCollisionShape;
    DebugHelper helper;

    PlayerStates playerState;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        projectGravity = (double) ProjectSettings.GetSetting("physics/2d/default_gravity");
        playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        playerCollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        playerSprite.Animation = "walk";

        
        playerState = PlayerStates.IDLE;

        helper = new DebugHelper();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // todo: figure out a better logging system
        helper.Run(delta);

        switch (playerState)
        {
            case PlayerStates.IDLE:
                timeDead = 0;
                RunAndJump(delta);
                break;
            case PlayerStates.DEAD:
                timeDead += delta;
                FallOverAndDie();
                break;
        }

        // debug print
        if (helper.intervalElapsed)
        {
            var strings = new List<string>();
            //strings.Add("direction input: " + directionVector.ToString());
            strings.Add("Velocity: " + Velocity.ToString());
            //helper.PrintAfterInterval(strings);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();

        // TODO: check if on the ground with ray tracing?
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

    private void FallOverAndDie()
    {
        Debug.Assert(deathHitDirection.LengthSquared() != 0, "death hit direction was not set");

        // TODO:
        double fallOverTime = 1;
        if ((fallOverTime < timeDead) && (!fallenOver))
        {
            // move to the ground
            // TODO: this assumes player has no rotation, and have rectangular collision box
            var collisionBox = (playerCollisionShape.Shape) as RectangleShape2D;
            if (collisionBox is not null)
            {
                var distanceToGroundAfterRotation = (collisionBox.Size[1] - collisionBox.Size[0]) / 2;
                Position = new Vector2(Position.X, Position.Y + distanceToGroundAfterRotation);
            }

            // rotate so the blood spews into the air, cuz it's funnier that way
            if (deathHitDirection.X < 0)
                Rotation += (float)-Math.PI/2;
            else
                Rotation += (float)Math.PI / 2;
            fallenOver = true;
        }

        // animation
        playerSprite.Animation = "dead";
    }

    #region IMortal Overrides
    public void OnHit(Vector2 hitPosition, Vector2 hitDirection)
    {
        HitPoints--;
        if ((HitPoints < 1) && (playerState != PlayerStates.DEAD))
        {
            deathHitDirection = hitDirection;
            Die();
        }

        if (playerState == PlayerStates.DEAD)
        {
            // TODO: could clean up this math a bit, make it simpler
            var bloodSpawner = BloodSpawnerScene.Instantiate<BloodSpawner>();
            bloodSpawner.Position = hitPosition - GlobalPosition;
            var hitAngle = (float)Math.Atan2((double)hitDirection[1], (double)hitDirection[0]);
            bloodSpawner.Rotate(hitAngle + (float)Math.PI);

            // player has maybe rotated, so need to translate based on this rotation
            var bsMagnitude = bloodSpawner.Position.Length();
            var bsAngle = Math.Atan2((double)bloodSpawner.Position.Y, (double)bloodSpawner.Position.X);
            bloodSpawner.Position = new Vector2((float)(bsMagnitude * Math.Cos(bsAngle - (double)GlobalRotation)), (float)(bsMagnitude * Math.Sin((double)bsAngle - (double)GlobalRotation)));
            bloodSpawner.Rotate(-Rotation);

            AddChild(bloodSpawner);
        }
    }

    public void Die()
    {
        playerState = PlayerStates.DEAD;
        Velocity = new Vector2(0, 0);
    }
    #endregion
}
