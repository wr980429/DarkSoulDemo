using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAnimFix : MonoBehaviour
{

    private Animator anim;
    public Vector3 a;
    private void Awake()
    {
        anim=GetComponent<Animator>();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (!anim.GetBool("Defense"))
        {  
            //���ֵ�С��
            var leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            leftLowerArm.localEulerAngles += 0.75f * a;
            anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftLowerArm.localEulerAngles));
        }
    }
}
