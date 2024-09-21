using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Bullet : Area2D
{
    float speed = 3000; // pixels per second
    float lifespan = 5; // seconds
    float distanceToTravel; // per frame
    float moveTolerance = 0.001f; // Don't want to move right up to surface, because after reflection might be stuck inside it
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
        if (Globals.PAUSE_ON)
            return;

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
        if (Globals.PAUSE_ON)
            return;

        // movement
        int i = 0;
        int maxCollisionsThisFrame = 10;
        while(distanceToTravel > 0 && i < maxCollisionsThisFrame)
        {
            MoveToNextCollision();
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

    private void MoveToNextCollision()
    {
        // ray trace
        var spaceState = GetWorld2D().DirectSpaceState;
        var from = Position;
        var to = Position + (direction.Normalized() * distanceToTravel);
        var query = PhysicsRayQueryParameters2D.Create(from, to);
        var result = spaceState.IntersectRay(query);

        // move to next surface, and either move through, reflect off, or despawn
        if (result.Count > 0)
        {
            StringName reflectorGroupName = new StringName(Globals.GROUP_REFLECTORS);

            var collidedNode = result["collider"].As<Node>();
            Debug.Assert(collidedNode is not null, "Collision with null");

            var collidedObject2d = collidedNode as CollisionObject2D;
            if (collidedObject2d is not null)
            {
                var collidedGroupNames = collidedObject2d.GetGroups();
                
                // 1 is general objects, 2 is player
                if (collidedObject2d.CollisionLayer > 2)
                {
                    MoveToCollisionSurface(result);
                    Position += 2 * moveTolerance * direction;
                }
                else if (collidedGroupNames.Contains(reflectorGroupName))
                    MoveToAndReflectOff(result);
                else if (collidedObject2d is IMortal mortal)
                {
                    // move up to object, but keep travelling same direction afterwards, and travel slightly extra, to get through body
                    MoveToCollisionSurface(result);
                    Position += 2 * moveTolerance * direction;

                    mortal.OnHit(Position, direction);
                }
                else
                {
                    // TODO: is some other kind of solid object, so move to it and despawn
                    MoveToCollisionSurface(result);
                    distanceToTravel = 0;
                    QueueFree();
                }
            }
            else
            {
                // TODO: doesn't have a collision object, but can still collide with it. So I think that's only tilemaps? So it's the ground, so despawn
                MoveToCollisionSurface(result);
                distanceToTravel = 0;
                QueueFree();
            }
        }
        // no collision
        else
        {
            Position += distanceToTravel * direction;
            distanceToTravel = 0;
        }
    }

    // reflect off a surface
    private void MoveToAndReflectOff(Godot.Collections.Dictionary collisionResult)
    {
        MoveToCollisionSurface(collisionResult);

        // reflect off
        var colliderNormal = (Vector2)collisionResult["normal"];
        var colliderAngle = Math.Atan2(colliderNormal.Y, colliderNormal.X);

        Rotation = (float)(Math.PI - Rotation) + (float)(2 * colliderAngle);
        direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
    }

    private void MoveToCollisionSurface(Godot.Collections.Dictionary collisionResult)
    {
        collisionCount += 1;

        // move up to the surface
        var distanceToCollision = (Position - (Vector2)collisionResult["position"]).Length();
        Debug.Assert(distanceToCollision <= distanceToTravel, "Collision outside of movement range");
        Position += ((distanceToCollision - moveTolerance) * direction);
        distanceToTravel -= distanceToCollision;
    }
}
