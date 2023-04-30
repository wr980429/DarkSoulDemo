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
    public float walkSpeed = 2.4f;  //用来调整动画的播放速度与实际的运行距离相匹配防止滑步
    public float runMulti = 2.7f;
    public float jumpVolecity = 4.0f;
    public float rollVolecity = 3f;
    public float jabMulti = 3f;

    [Space(10)]
    [Header("=====摩擦力设置====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;
    //冲量向量 用来给刚体施加向上的力
    private Vector3 thrustVec;
    //锁死水平方向的移动 不更新planarVec(否则一条就会使得数值更新为0 使得起跳后无法前进) 保留跳跃前的数值用来保持水平方向的遗留数值
    private bool isLockPanerVec = false;
    //追踪方向 为true时 追中Paner方向
    private bool trackDirection = false;
    //尽量不要加这种类型的信号
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

            //跳跃设计:按跳跃时 若Forward参数小于0.1就做后跳，0.1到1.1则是滚动,1.1到2是跳跃
            //利用线性插值来过渡动画
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
                //使用球形插值来平滑过渡
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
        //直接计算位置，并且赋值来实现移动
        //rigid.position += moveingVec * Time.fixedDeltaTime;

        //直接指派速度就不用乘以时间，但是注意velocity包含3个方向的力，如果在垂直方向没有设置力，那就会导致坡度往回走会凌空
        //正确写法见未注释
        //rigid.velocity = moveingVec;
        rigid.position += deltaPos;
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec; //使用刚体原本的Y方向的力
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
    #region 动画事件通知

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
        //这样方向一直是世界空间的-z方向
        //thrustVec = new Vector3(0,0, -jabVolecity);

        //取人物面向的反方向  但是这样设置后，单帧FixUpdate只会应用一次，人物的后跳移动会跟抽筋一样 所以在下面的OnJabUpdate中不停更新
        //thrustVec = playerPrefab.transform.forward * (-jabVolecity);
    }
    public void OnJabUpdate()
    {
        //后跳应该是一个逐渐缓动的动作 所以配合Curve去改善
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

        //lerp插值去设置layout的权重
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
        //第三段攻击动画读取动画的动作
        if(CheckState("attack1hC", "Attack"))
        {
            //降低更新的权重来减少摄像机的晃动  但是牺牲了动画运动的精准性
            deltaPos += (0.8f*deltaPos+0.2f*deltaPosition);
        }     
    }
    #endregion

    #endregion



}
