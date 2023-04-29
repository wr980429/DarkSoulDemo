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
    //��Ҫ����update�������׷�� ��Ȼ�������Ļ��������
    void FixedUpdate()
    {
        var tempModelEuler = model.transform.eulerAngles;
        //��������ϵ
        playerHandle.transform.Rotate(Vector3.up,pInput.JRight* horizontalSpped * Time.fixedDeltaTime);

        //������ת����������Ҫ������ת�ǶȵĻ����޷�ʹ�ø�ֵ 
        //cameraHandle.transform.Rotate(Vector3.up, pInput.JUp * -vecticalSpeed * Time.deltaTime);
        //tempEulerX =cameraHandle.transform.eulerAngles.x;

        tempEulerX -= pInput.JUp * vecticalSpeed * Time.fixedDeltaTime;

        //���ı����Լ���ŷ����localEulerAngles ������EulerAngles
        cameraHandle.transform.localEulerAngles = new Vector3(Mathf.Clamp(tempEulerX,-40,30),0,0);

        model.transform.eulerAngles = tempModelEuler;

        //ʹ��SmoothDampȥ�Ż�׷��
        //camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position,0.2f);
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position,ref cameraDampVelocity, 0.05f);
        camera.transform.eulerAngles  =transform.eulerAngles;
    }
}
