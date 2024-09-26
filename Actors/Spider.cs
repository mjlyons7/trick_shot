using Godot;
using System;

public partial class Spider : StaticBody2D, IMortal
{
    int _hitPoints = 1;
    public int HitPoints
    {
        get => _hitPoints;
        set => _hitPoints = value;
    }

    AnimatedSprite2D spiderSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        spiderSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        spiderSprite.Play();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    #region IMortal Overrides
    public void Die()
    {
        QueueFree();
    }
    #endregion
}
