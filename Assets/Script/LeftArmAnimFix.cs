using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAnimFix : MonoBehaviour
{

    private Animator anim;
    public Vector3 a;
    private ActorController ac;
    private void Awake()
    {
        anim=GetComponent<Animator>();
        ac=GetComponentInParent<ActorController>(); 
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (ac.leftIsShield)
        {
            if (!anim.GetBool("Defense"))
            {
                //×óÊÖµÄÐ¡±Û
                var leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                leftLowerArm.localEulerAngles += 0.75f * a;
                anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftLowerArm.localEulerAngles));
            }
        }    
    }
}
