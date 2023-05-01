using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UserInputBase : MonoBehaviour
{
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
    //protected bool isLastJump;

    public bool isMouse0;
    public bool isMouse1;
    // protected bool isLastAttack;
    public bool isCapsDown;

    public bool isDefense;
    public bool isRoll;
    public bool isLock;
    [Header("����")]
    protected  float targetDup;
    protected  float targetDright;
    protected  float velocityDup;
    protected  float velocityDright;
    public bool inputEanbled = true;

    protected Vector2 SquareToCircle(Vector2 input)
    {
        var outPut = Vector2.zero;
        outPut.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        outPut.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return outPut;
    }
    protected void UpdateDmagDvec(float Dup,float Dright)
    {
        Dmag = Mathf.Sqrt(Dup * Dup + Dright * Dright);
        Dvec = Dright * transform.right + Dup * transform.forward;
    }
}   
