using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class PlayerInput : MonoBehaviour
{
    [Header("��������")]
    public string KeyUp="w";
    public string KeyDown="s";
    public string KeyLeft="a";
    public string KeyRight="d";

    public string KEY_A="left shift";
    public string KEY_B;
    public string KEY_C;
    public string KEY_D;

    [Header("����ź�")]
    public float Dup;
    public float Dright;
    public float Dmag;
    public Vector3 Dvec;    //ǰ������

    //----------��ͷ�����÷�������------------///
    public float JUp;
    public float JRight;

    public bool isRun;
    public bool isJump;
    private bool isLastJump;

    public bool isAttack;
    public bool isLastAttack;

    [Header("����")]
    private float targetDup;
    private float targetDright;
    private float velocityDup;
    private float velocityDright;
    public bool inputEanbled = true;

    private void Update()
    {
        JUp= (Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1.0f : 0);
        JRight = (Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1.0f : 0);

        targetDup = (Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown)?1.0f:0);
        targetDright = (Input.GetKey(KeyRight) ? 1.0f : 0) - (Input.GetKey(KeyLeft) ? 1.0f : 0);

        if (!inputEanbled)
        {
            targetDup = 0;
            targetDright = 0;
        }
        //������������ֵ�ܹ���0.1f�ڴ�Dup��ΪtargetDup�����������ֵ��¼��velocityDup��
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        var tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        var Dright2 = tempDAxis.x;
        var Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt(Dup2 * Dup2 + Dright2 * Dright2);
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;

        isRun = Input.GetKeyDown(KeyCode.LeftShift)?!isRun:isRun;

        //�о����Ż��ռ�
        var tempJump=Input.GetKey(KeyCode.Space);
        isJump = tempJump != isLastJump && tempJump;
        isLastJump= tempJump;

        var tempAttack= Input.GetKey(KeyCode.Mouse0);
        isAttack = tempAttack != isLastAttack && tempAttack;
        isLastAttack = tempAttack;

    }

    private Vector2 SquareToCircle(Vector2 input)
    {
        var outPut = Vector2.zero;
        outPut.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        outPut.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return outPut;
    }
}
