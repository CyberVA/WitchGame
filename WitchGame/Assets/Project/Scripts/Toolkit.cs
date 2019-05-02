using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Toolkit
{
    
}

public class Timer
{
    // Floats
    float t;     //current time
    float sp;    //speed

    //Booleans
    bool tog;    //toggle

    /// <summary>
    /// Constructor for the Timer script, define your variables here homie
    /// </summary>
    /// <param name="speed">Speed of the timer, use 1 for seconds</param>
    /// <param name="toggle">toggles the timer on and off</param>
    public Timer(float speed, bool toggle)
    {
        sp = speed;
        tog = toggle;
        t = 0;
    }

    /// <summary>
    /// Runs the timer script
    /// </summary>
    /// <param name="deltaTime">time since last update</param>
    public void Update(float deltaTime)
    {
        //if tog is true, start adding deltatime multiplied by sp (speed) to t
        if(tog) t += deltaTime * sp;
    }

    /// <summary>
    /// Restarts the timer, sets t equal to 0
    /// </summary>
    public void Restart()
    {
        //resets t to 0
        t = 0;
    }
}
