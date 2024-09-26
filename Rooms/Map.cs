using Godot;
using System;
using System.Diagnostics;

public partial class Map : Node2D
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("reset"))
            GetTree().ReloadCurrentScene();
        // toggle pause
        // TODO: GetTree().Paused = true;
        else if (Input.IsActionJustPressed("pause"))
        {
            var tree = GetTree();
            tree.Paused = !tree.Paused;
            Globals.PAUSE_ON = tree.Paused;
        }
        
    }
}
