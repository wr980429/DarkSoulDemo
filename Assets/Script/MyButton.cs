using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton
{
    public bool IsPressing = false;
    public bool IsExtending = false;
    public bool IsDelaying = false;
    //双击后长按标志位
    public bool IsExtendingDelaying = false;
    public bool OnPressed = false;
    public bool OnReleased = false;


    private bool curState = false;
    private bool lastState = false;

    private MyTimer extTimer=new MyTimer();

    private MyTimer delayTimer = new MyTimer();

    //双击之后是否还是长按
    private MyTimer extDelayTimer=new MyTimer();

    public float extendingDuration = 0.15f;

    public float deylayingDuration = 0.15f;

    public float extendingDeylayDuration = 0.3f;

    public void Tick(bool input,float dt)
    {
        //StartTimer(extTimer, extendingDuration);
        extTimer.Tick(dt);
        delayTimer.Tick(dt);

        curState = input;
        IsPressing = curState;

        OnPressed = false;
        OnReleased= false; 

        if (curState != lastState)
        {
            if (curState)
            {
                OnPressed= true;
                //按下按键后开始计时
                StartTimer(delayTimer, deylayingDuration);
            }
            else
            {
                OnReleased = true;
                IsExtendingDelaying = false;
                //松开按键后开始计时
                StartTimer(extTimer, extendingDuration);
            }
        }
        lastState= curState;

        //计时内假如有再次输入则代表在规定的双击判断时间(extendingDuration)内又按下了按键，判断此次操作为双击
        //表示松开了该键，并且正在读键盘的后续持续输入正在判断是否可能在延迟时间区域时间内又按下了一次
        IsExtending = extTimer.state == MyTimer.STATE.RUN;
        //表示按下了该键，并且正在读键盘的后续持续输入正在判断是否为长按
        IsDelaying = delayTimer.state == MyTimer.STATE.RUN;
        if(IsExtending && IsDelaying)
            IsExtendingDelaying = true;        
    }

    public void StartTimer(MyTimer timer,float duration)
    {
        timer.durationTime = duration;
        timer.Go();
    }
}
