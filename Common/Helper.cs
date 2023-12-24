using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class DebugHelper
{
    public double fps;
    public double totalSecondsPassed;

    private double printInterval; // default 1 second
    public bool intervalElapsed;

    private double timeSinceLastInterval;
    private int framesSinceLastInterval;

    public DebugHelper(double printInterval = 1)
    {
        this.printInterval = printInterval;
    }

    // should be called every frame
    public void Run(double delta)
    {
        SetTimeVars(delta);
    }

    // call every frame, and will print when the set interval of time has elapsed
    public void PrintAfterInterval(IEnumerable<string> strings = null)
    {
        if (!intervalElapsed)
            return;
        intervalElapsed = false;

        if (strings is not null)
        {
            foreach (string varstr in strings) 
            {
                Debug.WriteLine(varstr);
            }
        }
    }

    // should be called very frame
    private void SetTimeVars(double delta)
    {
        // calculate fps, updated every second
        totalSecondsPassed += delta;

        // determine if the set interval of time has elapsed this frame or not
        timeSinceLastInterval += delta;
        framesSinceLastInterval += 1;
        if (timeSinceLastInterval > printInterval)
        {
            fps = framesSinceLastInterval / timeSinceLastInterval;
            timeSinceLastInterval = 0;
            framesSinceLastInterval = 0;
            intervalElapsed = true;
        }
    }
}
