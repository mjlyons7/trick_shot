using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Bullet : Area2D
{
    float speed = 1000; // pixels per second
    float lifespan = 5; // seconds
    float distanceToTravel; // per frame
    Vector2 direction;

    Sprite2D raySprite;
    DebugHelper helper;

    int collisionCount;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));

        // draw ray, for debugging
        raySprite = GetNode<Sprite2D>("ray");

        if (Globals.DEBUG_ON)
            lifespan = 60;

        helper = new DebugHelper();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        int speedModifier = 1;

        // low framerate mode, debug menu
        if (Globals.DEBUG_ON)
            if (!Input.IsActionJustPressed("debug_advance"))
                speedModifier = 0;

        // x = x0 + vt
        distanceToTravel += ((float)delta * speed * speedModifier);

        //animation
        if (Globals.DEBUG_ON)
        {
            if (distanceToTravel > 0)
            {
                raySprite.Visible = true;
                raySprite.Scale = new Vector2(distanceToTravel, 2);
                raySprite.Position = new Vector2(20 / 2, 0);
            }
        }

        // kill
        if (lifespan < helper.totalSecondsPassed)
        QueueFree();


        // logging
        var strings = new List<string>();
        //strings.Add("Distance to travel: " + distanceToTravel.ToString());
        //helper.PrintAfterInterval(strings);
        helper.Run(delta);
    }

    public override void _PhysicsProcess(double delta)
    {

        // movement
        int i = 0;
        int maxCollisionsThisFrame = 10;
        while(distanceToTravel > 0 && i < maxCollisionsThisFrame)
        {
            CheckForCollisions();
            i++;
        }

        if (i == maxCollisionsThisFrame)
        {
            var strings = new List<string>()
                {
                    "Hit max collision count"
                };
            helper.PrintAfterInterval(strings);
            distanceToTravel = 0;
        }

    }

    private void CheckForCollisions()
    {
        // detect collision surface with ray trace
        var spaceState = GetWorld2D().DirectSpaceState;
        var from = Position;
        var to = Position + (direction.Normalized() * distanceToTravel);
        var query = PhysicsRayQueryParameters2D.Create(from, to);
        var result = spaceState.IntersectRay(query);
        if (result.Count > 0)
        {
            StringName reflectorGroupName = new StringName(Globals.GROUP_REFLECTORS);

            var collidedNode = result["collider"].As<Node>();

            if (collidedNode is not null)
            {
                var collidedGroupNames = collidedNode.GetGroups();

                // decide what to do based on object collided with. Default case is despawn
                if (collidedGroupNames.Contains(reflectorGroupName))
                    MoveToAndReflectOff(result);
                else if (collidedNode is IMortal mortal)
                {
                    // move up to object, but keep travelling afterwards
                    Position += distanceToTravel * direction;
                    mortal.OnHit();
                }
                else
                {
                    Position += distanceToTravel * direction;
                    distanceToTravel = 0;
                    QueueFree();
                }
            }
        }
        // no collision
        else
        {
            // TODO: shouldn't have this code 3 times...
            Position += distanceToTravel * direction;
            distanceToTravel = 0;
        }
    }

    // reflect off a surface
    private void MoveToAndReflectOff(Godot.Collections.Dictionary collisionResult)
    {
        collisionCount += 1;
        float moveTolerance = 0.001f; // Don't want to move right up to surface, because after reflection might be stuck inside it

        // just for debugging
        var colliderId = (string)collisionResult["collider_id"];

        // move up to the surface
        var distanceToCollision = (Position - (Vector2)collisionResult["position"]).Length();
        Debug.Assert(distanceToCollision <= distanceToTravel);
        Position += ((distanceToCollision - moveTolerance) * direction);
        distanceToTravel -= distanceToCollision;

        // reflect off
        var colliderNormal = (Vector2)collisionResult["normal"];
        var colliderAngle = Math.Atan2(colliderNormal.Y, colliderNormal.X);

        Rotation = (float)(Math.PI - Rotation) + (float)(2 * colliderAngle);
        direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
    }
}
