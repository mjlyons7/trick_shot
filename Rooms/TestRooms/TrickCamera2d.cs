using Godot;
using System;
using System.Diagnostics;

public partial class TrickCamera2d : Camera2D
{

    CharacterBody2D player;
    
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //player = GetNode<CharacterBody2D>("Player");
        var playerNodes = GetTree().GetNodesInGroup("player");

        Debug.Assert(playerNodes.Count != 1, "Player count must be 1");
        player = playerNodes[0] as CharacterBody2D;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (player is not null)
            Position = player.Position;
    }
}
