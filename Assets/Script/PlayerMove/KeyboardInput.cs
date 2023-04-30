using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class KeyboardInput : UserInputBase
{
    [Header("��������")]
    public string KeyUp = "w";
    public string KeyDown = "s";
    public string KeyLeft = "a";
    public string KeyRight = "d";

    public string KEY_A = "left shift";
    public string KEY_B;
    public string KEY_C;
    public string KEY_D;

    public MyButton buttonJump = new MyButton();
    public MyButton buttonDefense = new MyButton();
    public MyButton buttonAttack = new MyButton();
    public MyButton buttonRun = new MyButton();
    public MyButton buttonForward = new MyButton();
    public MyButton buttonLock=new MyButton();

    [Header("=====�������=====")]
    public bool mouseEnable = true;
    public float mouseSensitivityX = 1f;
    public float mouseSensitivityY = 1f;

    private void Update()
    {
        var dt=Time.deltaTime;
        buttonJump.Tick(Input.GetKey(KeyCode.Space), dt);
        buttonDefense.Tick(Input.GetKey(KeyCode.Mouse1), dt);
        buttonAttack.Tick(Input.GetKey(KeyCode.Mouse0), dt);
        buttonRun.Tick(Input.GetKey(KeyCode.LeftShift), dt);
        buttonForward.Tick(Input.GetKey(KeyCode.W), dt);
        buttonLock.Tick(Input.GetKey(KeyCode.Tab), dt);


        if (mouseEnable)
        {
            //ʹ���������ӽ�
            JUp = Input.GetAxis("Mouse Y") * 2.5f * mouseSensitivityY;
            JRight = Input.GetAxis("Mouse X") * 2.5f * mouseSensitivityX;
        }
        else
        {
            //������Ҫ���ӽǼ��Ļ� ����ȥ��
            JUp = (Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1.0f : 0);
            JRight = (Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1.0f : 0);
        }


        targetDup = (Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown) ? 1.0f : 0);
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

        Dmag = Mathf.Sqrt(Dup2 * Dup2
            + Dright2 * Dright2);
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;

        //���ܼ������²��� && ˫��W������ס
        isRun = buttonRun.IsPressing || buttonForward.IsExtendingDelaying;
        isJump = buttonJump.OnPressed;// && buttonJump.IsExtending;
        isAttack = buttonAttack.OnPressed;
        isDefense = buttonDefense.IsPressing;
        //��Ծ���ɿ���ʱ���ڳ����ж����ӳ�������(0.15s),��������ʱ��ܶ� ��Ҫ����
        isRoll = buttonJump.OnReleased && buttonJump.IsDelaying;
        isLock=buttonLock.OnPressed;
    }
}