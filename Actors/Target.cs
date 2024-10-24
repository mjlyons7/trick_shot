using Godot;
using System;

public partial class Target : StaticBody2D, IMortal
{
    int _hitPoints = 1;
    public int HitPoints
    {
        get => _hitPoints;
        set => _hitPoints = value;
    }

    AnimatedSprite2D targetSprite;
    CollisionShape2D hitbox;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        targetSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        hitbox = GetNode<CollisionShape2D>("CollisionShape2D");
        targetSprite.Animation = "default";
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    void OnAnimationFinished()
    {
        QueueFree();
    }


    public void Die()
    {
        targetSprite.Animation = "explode";
        targetSprite.Play();
        hitbox.QueueFree();
    }
}
