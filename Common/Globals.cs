using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Globals
{
    public static bool DEBUG_ON = false;
    public static bool PAUSE_ON = false;

    // group string names
    // TODO: there's a better way to do this
    public const string GROUP_PLAYER = "player";
    public const string GROUP_REFLECTORS = "reflectors";
}
