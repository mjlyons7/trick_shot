using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class Bullet : Area2D
{
	float speed = 1000; // pixels per second
	float rayLength = 25;
	float lifespan = 5; // seconds
	Vector2 direction;
	DebugHelper helper;

	int collisionCount;
	string previousAreaCollidedWith;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));

		// draw ray, for debugging
		var raySprite = GetNode<Sprite2D>("ray");
		if (Globals.DEBUG_ON)
        {
			raySprite.Scale = new Vector2(rayLength, 2);
			raySprite.Position = new Vector2(rayLength / 2, 0);
			lifespan = 60;
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
		int speedModifier = 1;

		// low framerate mode, debug menu
		if (Globals.DEBUG_ON)
			if (!Input.IsActionJustPressed("debug_advance"))
				speedModifier = 0;

        // x = x0 + vt
        Position += ((float)delta * speed * speedModifier) * direction;

		helper.Run(delta);

		// kill
		if (lifespan < helper.totalSecondsPassed)
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
			StringName reflectorGroupName = new StringName(Globals.GROUP_REFLECTORS);

            var collidedNode = result["collider"].As<Node>();

            if (collidedNode is not null)
			{
				var collidedGroupNames = collidedNode.GetGroups();

				// decide what to do based on object collided with. Default case is despawn
				if (collidedGroupNames.Contains(reflectorGroupName))
					ReflectOff(result);
				else if (collidedNode is IMortal mortal)
				{
					// TODO: should be pub/sub model, I think
					mortal.OnHit();
                }
				else
					QueueFree();
			}
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
