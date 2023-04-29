using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject playerPrefab;
    public PlayerInput pInput;
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
    //尽量不要加这种类型的信号
    private bool canAttack;

    private CapsuleCollider col;


    void Awake()
    {
        playerPrefab = this.transform.Find("ybot").gameObject;
        pInput = GetComponent<PlayerInput>();
        anim = this.transform.Find("ybot").GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();     
    }

    // Timer.DeltaTime 1/60
    void Update()
    {

        //跳跃设计:按跳跃时 若Forward参数小于0.1就做后跳，0.1到1.1则是滚动,1.1到2是跳跃

        //利用线性插值来过渡动画
        anim.SetFloat("Forward", pInput.Dmag * Mathf.Lerp(anim.GetFloat("Forward"), (pInput.isRun ? 2.0f : 1.0f), 0.5f));
        if (rigid.velocity.magnitude > 1.0f)
        {
            anim.SetTrigger("Roll");
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

    //Timer.DeltaTime 1/50
    private void FixedUpdate()
    {
        //直接计算位置，并且赋值来实现移动
        //rigid.position += moveingVec * Time.fixedDeltaTime;

        //直接指派速度就不用乘以时间，但是注意velocity包含3个方向的力，如果在垂直方向没有设置力，那就会导致坡度往回走会凌空
        //正确写法见未注释
        //rigid.velocity = moveingVec;
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec; //使用刚体原本的Y方向的力
        thrustVec = Vector3.zero;
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
    }
    public void OnFallEnter()
    {
        SetPanerVec(true);
    }

    public void OnRollEnter()
    {
        SetPanerVec(true);
        thrustVec = new Vector3(0, rollVolecity, 0);
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
        anim.SetLayerWeight(anim.GetLayerIndex("Attack"), 1.0f);
    }
    public void OnAttack1hAUpdate()
    {
        thrustVec = playerPrefab.transform.forward * anim.GetFloat("Attack1hAVelocity");
    }
    public void OnAttackIdle()
    {
        SetPanerVec(false);
        anim.SetLayerWeight(anim.GetLayerIndex("Attack"), 0f);
    }
    public void OnGroundExit()
    {
        col.material = frictionZero;
    }
    #endregion



}
