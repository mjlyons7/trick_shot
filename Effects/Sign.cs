using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class Sign : Node2D
{
	[Export]
	PackedScene textboxScene;

	[Export]
	PackedScene speechBubbleScene;

	[Export]
	string signText;

	static Textbox signTextbox;
	Node2D speechBubble;

	bool playerInsideSignArea;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (signTextbox is null)
		{
			signTextbox = textboxScene.Instantiate<Textbox>();
			AddChild(signTextbox);
		}

        speechBubble = speechBubbleScene.Instantiate<Node2D>();
		AddChild(speechBubble);

		speechBubble.Visible = false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        // if player inside area, then look for "enter" input, and if get it, load text box
        if (playerInsideSignArea && !signTextbox.readingInProgress)
        {
            if (Input.IsActionJustPressed("enter"))
            {
				signTextbox.AddText(signText);
            }
        }

		speechBubble.Visible = (playerInsideSignArea && !signTextbox.readingInProgress);
    }

	// signal
	void OnBodyEntered(PhysicsBody2D bodyEntering)
	{
		StringName playerGroupName = Globals.GROUP_PLAYER;
		if (bodyEntering.GetGroups().Contains<StringName>(playerGroupName))
		{
            playerInsideSignArea = true;
        }
    }

	void OnBodyExited(PhysicsBody2D bodyExiting)
	{
        StringName playerGroupName = Globals.GROUP_PLAYER;
		if (bodyExiting.GetGroups().Contains<StringName>(playerGroupName))
		{
            playerInsideSignArea = false;
        }
    }
}
