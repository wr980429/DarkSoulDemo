using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject playerPrefab;
    public PlayerInput pInput;
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
    //������Ҫ���������͵��ź�
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

        //��Ծ���:����Ծʱ ��Forward����С��0.1����������0.1��1.1���ǹ���,1.1��2����Ծ

        //�������Բ�ֵ�����ɶ���
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
            //ʹ�����β�ֵ��ƽ������
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
        //ֱ�Ӽ���λ�ã����Ҹ�ֵ��ʵ���ƶ�
        //rigid.position += moveingVec * Time.fixedDeltaTime;

        //ֱ��ָ���ٶȾͲ��ó���ʱ�䣬����ע��velocity����3���������������ڴ�ֱ����û�����������Ǿͻᵼ���¶������߻����
        //��ȷд����δע��
        //rigid.velocity = moveingVec;
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec; //ʹ�ø���ԭ����Y�������
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
