using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : IActorManager
{
    //public ActorManager am;
    public float HP = 15f;
    public float HPMax = 15f;

    public bool isGround;
    public bool isJump;
    public bool isFall;
    public bool isRoll;
    public bool isDefense;
    public bool isAttack;
    public bool isBlocked;
    public bool isHit;
    public bool isDead;
    public bool isJab;
    public bool isCounterBack=false; //是否在做挥舞盾的动作
    public bool isCounterBackEnable; //是否在挥舞盾的有效时间内


    public bool isAllowDefense;
    public bool isImmortal;
    public bool isCounterBackSuccess;
    public bool isCounterBackFailure;
    private void Start()
    {
        HP = HPMax;
    }
    public void Update()
    {
        isGround = am.ac.CheckState("Ground");
        isJump = am.ac.CheckState("jump");
        isFall = am.ac.CheckState("Falling Idle");
        isRoll = am.ac.CheckState("Roll");

        isAttack = am.ac.CheckStateTag("AttackL") || am.ac.CheckStateTag("AttackR");
        isBlocked = am.ac.CheckState("blocked");
        isHit = am.ac.CheckState("hit");
        isDead = am.ac.CheckState("dead");
        isJab = am.ac.CheckState("jab");

        isAllowDefense = isGround || isBlocked;
        isDefense = isAllowDefense && am.ac.CheckState("defense1h", "Defense");
        isCounterBack = am.ac.CheckState("counterBack");
        isImmortal = isRoll || isJab;

        isCounterBackSuccess = isCounterBackEnable;
        isCounterBackFailure= !isCounterBackEnable && isCounterBack;
    }

    public void AddHp(float value)
    {
        HP += value;
        HP=Mathf.Clamp(HP,0,HPMax);
    }
}
