using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton
{
    public bool IsPressing = false;
    public bool IsExtending = false;
    public bool IsDelaying = false;
    //˫���󳤰���־λ
    public bool IsExtendingDelaying = false;
    public bool OnPressed = false;
    public bool OnReleased = false;


    private bool curState = false;
    private bool lastState = false;

    private MyTimer extTimer=new MyTimer();

    private MyTimer delayTimer = new MyTimer();

    //˫��֮���Ƿ��ǳ���
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
                //���°�����ʼ��ʱ
                StartTimer(delayTimer, deylayingDuration);
            }
            else
            {
                OnReleased = true;
                IsExtendingDelaying = false;
                //�ɿ�������ʼ��ʱ
                StartTimer(extTimer, extendingDuration);
            }
        }
        lastState= curState;

        //��ʱ�ڼ������ٴ�����������ڹ涨��˫���ж�ʱ��(extendingDuration)���ְ����˰������жϴ˴β���Ϊ˫��
        //��ʾ�ɿ��˸ü����������ڶ����̵ĺ����������������ж��Ƿ�������ӳ�ʱ������ʱ�����ְ�����һ��
        IsExtending = extTimer.state == MyTimer.STATE.RUN;
        //��ʾ�����˸ü����������ڶ����̵ĺ����������������ж��Ƿ�Ϊ����
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
