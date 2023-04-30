using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTimer
{
    public enum STATE
    {
        IDLE,
        RUN,
        FINISHED
    }
    public STATE state;
    public float durationTime = 1.0f;
    public float elapsedTime = 0.0f;

    public void Tick(float dt)
    {
        if (state == STATE.IDLE)
        {

        }
        else if (state == STATE.RUN)
        {
            elapsedTime += dt;
            if (elapsedTime > durationTime)
            {
                state = STATE.FINISHED;
            }
        }
        else if (state == STATE.FINISHED) 
        { 

        }
    }
    public void Go()
    {
        elapsedTime = 0;
        state = STATE.RUN;
    }
}
