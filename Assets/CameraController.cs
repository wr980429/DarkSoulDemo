using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public PlayerInput pInput;
    public float horizontalSpped=100f;
    public float vecticalSpeed = 80f;
    private GameObject playerHandle;
    private GameObject cameraHandle;
    private GameObject model;
    private Camera camera;
    private float tempEulerX;
    private Vector3 cameraDampVelocity;
    private void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 20;
        model = playerHandle.GetComponent<ActorController>().playerPrefab;
        camera = Camera.main;
    }
    //不要放在update里面更新追踪 不然会出现屏幕抖动问题
    void FixedUpdate()
    {
        var tempModelEuler = model.transform.eulerAngles;
        //左手坐标系
        playerHandle.transform.Rotate(Vector3.up,pInput.JRight* horizontalSpped * Time.fixedDeltaTime);

        //这样旋转会出现如果想要限制旋转角度的话，无法使用负值 
        //cameraHandle.transform.Rotate(Vector3.up, pInput.JUp * -vecticalSpeed * Time.deltaTime);
        //tempEulerX =cameraHandle.transform.eulerAngles.x;

        tempEulerX -= pInput.JUp * vecticalSpeed * Time.fixedDeltaTime;

        //更改本地自己的欧拉角localEulerAngles 而不是EulerAngles
        cameraHandle.transform.localEulerAngles = new Vector3(Mathf.Clamp(tempEulerX,-40,30),0,0);

        model.transform.eulerAngles = tempModelEuler;

        //使用SmoothDamp去优化追踪
        //camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position,0.2f);
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position,ref cameraDampVelocity, 0.05f);
        camera.transform.eulerAngles  =transform.eulerAngles;
    }
}
