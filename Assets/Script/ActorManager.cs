using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public BattleManager bm;
    public WeaponManager wm;
    public StateManager sm;


    public ActorController ac;
    private void Awake()
    {
        ac=GetComponent<ActorController>(); 
        var sensor = transform.Find("Sensor");

        bm = Bind<BattleManager>(sensor.gameObject);
        wm = Bind<WeaponManager>(ac.playerPrefab);
        sm = Bind<StateManager>(gameObject);

    }
    private T Bind<T>(GameObject go) where T : IActorManager
    {
        T tempInstance;
        tempInstance= go.GetComponent<T>(true);
        tempInstance.am = this;
        return tempInstance;
    }
    public void TryDoDmage(WeaponController targetWc,bool attackValid,bool counterValid)
    {

        if (sm.isCounterBackSuccess)
        {
            if(counterValid)
                targetWc.wm.am.Stunned();
        }
        else if (sm.isCounterBackFailure)
        {
            if(attackValid)
                HitOrDie(false);
        }
        else if (sm.isImmortal)
        {
            //Do nothing
        }
        else if (sm.isDefense &&  attackValid)
        {
            Blocked();
        }
        else
        {
            if (attackValid)
                HitOrDie(true);
        }
    }

    private void HitOrDie(bool doHitAnimation)
    {
        if (sm.HP <= 0)
        {

        }
        else
        {
            sm.AddHp(-5);
            if (sm.HP > 0)
            {
                //盾反失败情况下被打了继续做完盾反动作不打断
                if(doHitAnimation)
                {
                    Hit();
                    //粒子特效等
                }
            }
            else
            {
                Die();
            }
        }
    }

    private void Stunned()
    {
        ac.IssuerTrigger("Stunned");
    }

    public void Blocked()
    {
        ac.IssuerTrigger("Blocked");
    }
    public void Hit()
    {
        ac.IssuerTrigger("Hit");
    }
    public void Die()
    {
        ac.IssuerTrigger("Die");
        ac.pInput.inputEanbled = false;
        if (ac.camControl.lockState)
        {
            ac.camControl.LockUnLock();          
        }
        ac.camControl.enabled = false;
    }
    public void SetIsCounterBack(bool value)
    {
        sm.isCounterBackEnable = value;
    }
}
