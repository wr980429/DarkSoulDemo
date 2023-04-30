using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider capCol;

    private Vector3 point1;
    private Vector3 point2;
    //碰撞用的胶囊体往下偏移一些，更早判断为动画做缓冲
    private float OffSet = 0.3f;
    private float radius;

    // Start is called before the first frame update
    void Awake()
    {
        radius = capCol.radius - 0.05f;
    }
    private void FixedUpdate()
    {
        //不停更新检测用的位置
        point1 = transform.position + transform.up * (radius - OffSet);
        point2 = transform.position + transform.up * (capCol.height - OffSet) - transform.up * radius;
        Collider[] outputCols = Physics.OverlapCapsule(point1, point2, radius, LayerMask.GetMask("Ground"));
        if (outputCols.Length != 0)
        {
            SendMessageUpwards("IsGround");
        }
        else
        {
            SendMessageUpwards("IsNotGround");
        }
    }
}
