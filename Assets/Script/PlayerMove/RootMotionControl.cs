using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RootMotionControl : MonoBehaviour
{
    private Animator anim;
    private ActorController actCtro;

    private void Awake()
    {
        anim= GetComponent<Animator>();
        actCtro=this.transform.parent.GetComponent<ActorController>();
    }
    private void OnAnimatorMove()
    {

        actCtro.OnUpdateRM(anim.deltaPosition);
    }
}
