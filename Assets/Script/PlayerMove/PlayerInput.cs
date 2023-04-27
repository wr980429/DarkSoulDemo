using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public string KeyUp="w";
    public string KeyDown="s";
    public string KeyLeft="a";
    public string KeyRight="d";

    public float Dup;
    public float Dright;

    private float targetDup;
    private float targetDright;

    private float velocityDup;
    private float velocityDright;

    public bool inputEanbled = true;

    private void Update()
    {
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
    }
}
