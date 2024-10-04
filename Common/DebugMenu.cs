using Godot;
using System;
using System.Collections.Generic;

public partial class DebugMenu : PopupMenu
{
    DebugHelper helper;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        helper = new DebugHelper();
        ProcessMode = ProcessModeEnum.Always;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // show menu when paused
        Visible = Globals.PAUSE_ON;

        helper.Run(delta);
    }

    // signal
    public void OnIdPressed(int id)
    {
        // id 0
        switch (id)
        {
            case 0:
                ToggleDebugMode(id);
                break;
            default:
                // todo: put error for unsupported menu option
                break;
        }
    }

    // ---- private ---
    private void ToggleDebugMode(int id)
    {
        // toggle checkbox on and off
        if (Globals.DEBUG_ON)
        {
            Globals.DEBUG_ON = false;
            this.SetItemChecked(id, false);
        }
        else
        {
            Globals.DEBUG_ON = true;
            this.SetItemChecked(id, true);
        }
    }
}
