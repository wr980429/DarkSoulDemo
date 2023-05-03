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
    public DirectorManager dm;
    public InteractionManager im;
    public EventCasterManager ecm;


    public ActorController ac;
    private void Awake()
    {
        ac=GetComponent<ActorController>(); 
        var sensor = transform.Find("Sensor");

        bm = Bind<BattleManager>(sensor.gameObject);
        wm = Bind<WeaponManager>(ac.playerPrefab);
        sm = Bind<StateManager>(gameObject);
        dm = Bind<DirectorManager>(gameObject);
        im= Bind<InteractionManager>(sensor.gameObject);

        ac.OnActionBtnClick += DoInteractAction;
    }

    private void DoInteractAction()
    {
        if (im.overlapEcasetms.Count != 0)
        {
            var firstEvent = im.overlapEcasetms[0];
            if (firstEvent.active)
            {
                if (firstEvent.eventName == "frontStab")
                {
                    dm.PlayFrontStab("frontStab", this, firstEvent.am);
                }
                else if (firstEvent.eventName == "openBox")
                {
                    if (BattleManager.CheckAnglePlayer(ac.playerPrefab, firstEvent.am.gameObject, 180))
                    {
                        firstEvent.active = false;
                        transform.position = firstEvent.am.transform.position + firstEvent.am.transform.TransformVector(firstEvent.offset);
                        ac.playerPrefab.transform.LookAt(firstEvent.am.transform,Vector3.up);
                        dm.PlayFrontStab("openBox", this, firstEvent.am);
                    }
                }
                else if (firstEvent.eventName == "leverUp")
                {
                    if (BattleManager.CheckAnglePlayer(ac.playerPrefab, firstEvent.am.gameObject, 180))
                    {
                        //firstEvent.active = false;
                        transform.position = firstEvent.am.transform.position + firstEvent.am.transform.TransformVector(firstEvent.offset);
                        ac.playerPrefab.transform.LookAt(firstEvent.am.transform, Vector3.up);
                        dm.PlayFrontStab("leverUp", this, firstEvent.am);
                    }
                }
            }
        }
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
    public void LockUnlockActorController(bool value)
    {
        ac.IssueBool("Lock", value);
    }
}
