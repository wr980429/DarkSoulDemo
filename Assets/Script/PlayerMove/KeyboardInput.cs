using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class KeyboardInput : UserInputBase
{
    [Header("键盘输入")]
    public string KeyUp = "w";
    public string KeyDown = "s";
    public string KeyLeft = "a";
    public string KeyRight = "d";

    public string KEY_A = "left shift";
    public string KEY_B;
    public string KEY_C;
    public string KEY_D;

    public MyButton buttonJump = new MyButton();
    //public MyButton buttonDefense = new MyButton();
    public MyButton buttonMouse0 = new MyButton();
    public MyButton buttonMouse1 = new MyButton();
    public MyButton buttonCaps = new MyButton();
    public MyButton buttonRun = new MyButton();
    public MyButton buttonForward = new MyButton();
    public MyButton buttonLock=new MyButton();
    public MyButton buttonAction = new MyButton();
    [Header("=====鼠标设置=====")]
    public bool mouseEnable = true;
    public float mouseSensitivityX = 1f;
    public float mouseSensitivityY = 1f;

    private void Update()
    {
        var dt=Time.deltaTime;
        buttonJump.Tick(Input.GetKey(KeyCode.Space), dt);
        //buttonDefense.Tick(Input.GetKey(KeyCode.Mouse0), dt);
        buttonMouse0.Tick(Input.GetKey(KeyCode.Mouse0), dt);
        buttonMouse1.Tick(Input.GetKey(KeyCode.Mouse1), dt);
        buttonRun.Tick(Input.GetKey(KeyCode.LeftShift), dt);
        buttonForward.Tick(Input.GetKey(KeyCode.W), dt);
        buttonLock.Tick(Input.GetKey(KeyCode.Tab), dt);
        buttonCaps.Tick(Input.GetKey(KeyCode.CapsLock), dt);
        buttonAction.Tick(Input.GetKey(KeyCode.F), dt);

        if (mouseEnable)
        {
            //使用鼠标控制视角
            JUp = Input.GetAxis("Mouse Y") * 2.5f * mouseSensitivityY;
            JRight = Input.GetAxis("Mouse X") * 2.5f * mouseSensitivityX;
        }
        else
        {
            //假如需要绑定视角键的话 这样去算
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
        //缓动函数，让值能够在0.1f内从Dup变为targetDup，具体的力度值记录在velocityDup上
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        var tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        var Dright2 = tempDAxis.x;
        var Dup2 = tempDAxis.y;
        UpdateDmagDvec(Dup2,Dright2);

        //奔跑键正按下不动 && 双击W键并按住
        isRun = buttonRun.IsPressing || buttonForward.IsExtendingDelaying;
        isJump = buttonJump.OnPressed;// && buttonJump.IsExtending;
        isMouse0 = buttonMouse0.OnPressed;
        isMouse1= buttonMouse1.OnPressed;
        isCapsDown=buttonCaps.IsPressing;
        isDefense = buttonMouse0.IsPressing && buttonCaps.IsPressing;
        //跳跃键松开的时候还在长按判定的延迟区间内(0.15s),代表按得时间很短 想要翻滚
        isRoll = buttonJump.OnReleased && buttonJump.IsDelaying;
        isLock=buttonLock.OnPressed;
        isAction = buttonAction.OnPressed;
       // isMouse0LongPress = buttonMouse0.IsDelaying;
    }
}
