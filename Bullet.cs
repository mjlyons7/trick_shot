using Godot;
using System;
using System.Diagnostics;

public partial class Bullet : Area2D
{
	float speed = 1000; // pixels per second
	float rayLength = 25;
	Vector2 direction;
	DebugHelper helper;

	int collisionCount;
	int maxCollisions = 5;
	string previousAreaCollidedWith;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));

		// draw ray, for debugging
		var raySprite = GetNode<Sprite2D>("ray");
		if (Globals.DEBUG_MODE)
        {
			raySprite.Scale = new Vector2(rayLength, 2);
			raySprite.Position = new Vector2(rayLength / 2, 0);
        }
        else
        {
			raySprite.Visible = false;
        }

		helper = new DebugHelper();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// x = x0 + vt
		Position += ((float)delta * speed) * direction;

		helper.Run(delta);

		// kill
		if (collisionCount > maxCollisions)
			QueueFree();
	}

    public override void _PhysicsProcess(double delta)
    {

		// detect collision surface with ray trace
		// TODO: only get results for reflectors
		var spaceState = GetWorld2D().DirectSpaceState;
		var from = this.Position;
		var to = this.Position + (direction.Normalized() * rayLength);
		var query = PhysicsRayQueryParameters2D.Create(from, to);
		var result = spaceState.IntersectRay(query);
		if (result.Count > 0)
        {
			ReflectOff(result);
        }
	}

    // reflect off a surface
    private void ReflectOff(Godot.Collections.Dictionary collisionResult)
    {
		collisionCount += 1;

		var colliderNormal = (Vector2)collisionResult["normal"];
		var colliderAngle = Math.Atan2(colliderNormal.Y, colliderNormal.X);

		Rotation = (float)(Math.PI - Rotation) + (float)(2 * colliderAngle);
		direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
	}
}
