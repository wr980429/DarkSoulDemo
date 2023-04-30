using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject playerPrefab;
    public CameraController camControl;
    public UserInputBase pInput;
    public float walkSpeed = 2.4f;  //�������������Ĳ����ٶ���ʵ�ʵ����о�����ƥ���ֹ����
    public float runMulti = 2.7f;
    public float jumpVolecity = 4.0f;
    public float rollVolecity = 3f;
    public float jabMulti = 3f;

    [Space(10)]
    [Header("=====Ħ��������====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;
    //�������� ����������ʩ�����ϵ���
    private Vector3 thrustVec;
    //����ˮƽ������ƶ� ������planarVec(����һ���ͻ�ʹ����ֵ����Ϊ0 ʹ���������޷�ǰ��) ������Ծǰ����ֵ��������ˮƽ�����������ֵ
    private bool isLockPanerVec = false;
    //׷�ٷ��� Ϊtrueʱ ׷��Paner����
    private bool trackDirection = false;
    //������Ҫ���������͵��ź�
    private bool canAttack;

    private CapsuleCollider col;

    private float lerpTarget;
    private Vector3 deltaPos;
    void Awake()
    {
        playerPrefab = this.transform.Find("ybot").gameObject;
        pInput = GetComponent<UserInputBase>();
        anim = this.transform.Find("ybot").GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        camControl= this.transform.Find("CameraHandle/CameraPos").GetComponent<CameraController>();
    }

    // Timer.DeltaTime 1/60
    void Update()
    {

        if (pInput.isLock)
        {
            camControl.LockUnLock();
        }


        if (!camControl.lockState)
        {

            //��Ծ���:����Ծʱ ��Forward����С��0.1����������0.1��1.1���ǹ���,1.1��2����Ծ
            //�������Բ�ֵ�����ɶ���
            anim.SetFloat("Forward", pInput.Dmag * Mathf.Lerp(anim.GetFloat("Forward"), (pInput.isRun ? 2.0f : 1.0f), 0.5f));
            anim.SetFloat("Right",0);
        }
        else
        {
            var localDvecz = transform.InverseTransformVector(pInput.Dvec);
            anim.SetFloat("Forward", localDvecz.z* (pInput.isRun ? 2.0f : 1.0f));
            anim.SetFloat("Right", localDvecz.x * (pInput.isRun ? 2.0f : 1.0f));
        }



        //if (pInput.isJump && rigid.velocity.magnitude > 1.0f)
        if (pInput.isRoll || rigid.velocity.magnitude > 7.0f)
        {
            anim.SetTrigger("Roll");
            canAttack = false;
        }
        if (pInput.isJump)
        {
            anim.SetTrigger("Jump");
            canAttack = false;
        }

        if (pInput.isAttack && CheckState("Ground") && canAttack)
        {
            anim.SetTrigger("Attack");
        }

        anim.SetBool("Defense", pInput.isDefense);

        if (!camControl.lockState)
        {
            if (pInput.Dmag > 0.1f)
            {
                //ʹ�����β�ֵ��ƽ������
                playerPrefab.transform.forward = Vector3.Slerp(playerPrefab.transform.forward, pInput.Dvec, 0.3f);
            }
            if (!isLockPanerVec)
            {
                planarVec = pInput.Dmag * playerPrefab.transform.forward * walkSpeed * (pInput.isRun ? runMulti : 1.0f);
            }
        }
        else
        {
            if (!trackDirection)
            {
                playerPrefab.transform.forward = transform.forward;
            }
            else
            {
                playerPrefab.transform.forward = planarVec.normalized;
            }
            if (!isLockPanerVec)
                planarVec = pInput.Dvec * walkSpeed * (pInput.isRun ? runMulti : 1.0f);
        }

    }

    //Timer.DeltaTime 1/50
    private void FixedUpdate()
    {
        //ֱ�Ӽ���λ�ã����Ҹ�ֵ��ʵ���ƶ�
        //rigid.position += moveingVec * Time.fixedDeltaTime;

        //ֱ��ָ���ٶȾͲ��ó���ʱ�䣬����ע��velocity����3���������������ڴ�ֱ����û�����������Ǿͻᵼ���¶������߻����
        //��ȷд����δע��
        //rigid.velocity = moveingVec;
        rigid.position += deltaPos;
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec; //ʹ�ø���ԭ����Y�������
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }
    private bool CheckState(string stateName, string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }
    public void SetPanerVec(bool isLock)
    {
        pInput.inputEanbled = !isLock;
        isLockPanerVec = isLock;
    }
    #region �����¼�֪ͨ

    //public void OnJumpExit()
    //{
    //    pInput.inputEanbled = true;
    //    isLockPanerVec = false;
    //}

    public void OnJumpEnter()
    {
        SetPanerVec(true);
        thrustVec = new Vector3(0, jumpVolecity);
        trackDirection = true;
    }
    public void IsGround()
    {
        anim.SetBool("IsGround", true);
    }
    public void IsNotGround()
    {
        anim.SetBool("IsGround", false);
    }
    public void OnGroundEnter()
    {
        SetPanerVec(false);
        canAttack = true;
        col.material = frictionOne;
        trackDirection= false;
    }
    public void OnGroundExit()
    {
        col.material = frictionZero;
    }
    public void OnFallEnter()
    {
        SetPanerVec(true);
    }

    public void OnRollEnter()
    {
        SetPanerVec(true);
        thrustVec = new Vector3(0, rollVolecity, 0);
        trackDirection= true; 
    }
    public void OnJabEnter()
    {
        SetPanerVec(true);
        //��������һֱ������ռ��-z����
        //thrustVec = new Vector3(0,0, -jabVolecity);

        //ȡ��������ķ�����  �����������ú󣬵�֡FixUpdateֻ��Ӧ��һ�Σ�����ĺ����ƶ�������һ�� �����������OnJabUpdate�в�ͣ����
        //thrustVec = playerPrefab.transform.forward * (-jabVolecity);
    }
    public void OnJabUpdate()
    {
        //����Ӧ����һ���𽥻����Ķ��� �������Curveȥ����
        //thrustVec = playerPrefab.transform.forward * (-jabVolecity);
        thrustVec = playerPrefab.transform.forward * anim.GetFloat("JabVelocity");
    }

    public void OnAttack1hAEnter()
    {
        SetPanerVec(true);
        isLockPanerVec = false;
        lerpTarget = 1.0f;   
    }
    public void OnAttack1hAUpdate()
    {
        thrustVec = playerPrefab.transform.forward * anim.GetFloat("Attack1hAVelocity");

        //lerp��ֵȥ����layout��Ȩ��
        var attackLayoutIndex = anim.GetLayerIndex("Attack");
        var currentWeight = anim.GetLayerWeight(attackLayoutIndex);
        currentWeight = Mathf.Lerp(currentWeight, lerpTarget,0.4f);
        anim.SetLayerWeight(attackLayoutIndex, currentWeight);
    }
    public void OnAttackIdleEnter()
    {
        SetPanerVec(false);
        //anim.SetLayerWeight(anim.GetLayerIndex("Attack"), 0f);
        lerpTarget = 0;
    }
    public void OnAttackIdleUpdate()
    {
        var attackLayoutIndex = anim.GetLayerIndex("Attack");
        var currentWeight = Mathf.Lerp(anim.GetLayerWeight(attackLayoutIndex), lerpTarget, 0.4f);
        anim.SetLayerWeight(attackLayoutIndex, currentWeight);
    }

    #region OnAnimtorMove
    public void OnUpdateRM(Vector3 deltaPosition)
    {
        //�����ι���������ȡ�����Ķ���
        if(CheckState("attack1hC", "Attack"))
        {
            //���͸��µ�Ȩ��������������Ļζ�  ���������˶����˶��ľ�׼��
            deltaPos += (0.8f*deltaPos+0.2f*deltaPosition);
        }     
    }
    #endregion

    #endregion



}
