using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCasterManager : IActorManager
{
    public string eventName;
    public bool active;
    public Vector3 offset=new Vector3(0,0,1f);
    private void Start()
    {
        if (am == null)
        { 
            am=GetComponentInParent<ActorManager>();        
        }
    }

}
