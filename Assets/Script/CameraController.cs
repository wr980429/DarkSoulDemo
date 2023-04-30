using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{

    public UserInputBase pInput;
    public float horizontalSpped = 100f;
    public float vecticalSpeed = 80f;
    public Image lockDot;
    public bool lockState;

    private GameObject playerHandle;
    private GameObject cameraHandle;
    private GameObject model;
    private Camera mainCamera;
    private float tempEulerX;
    private Vector3 cameraDampVelocity;

    private LockTarget lockTarget;
    private void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 20;
        model = playerHandle.GetComponent<ActorController>().playerPrefab;
        mainCamera = Camera.main;
        lockDot.enabled = false;
        lockState = false;
        lockTarget = new LockTarget();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    //��Ҫ����update�������׷�� ��Ȼ�������Ļ��������
    void FixedUpdate()
    {
        if (lockTarget.obj == null)
        {
            var tempModelEuler = model.transform.eulerAngles;
            //��������ϵ
            playerHandle.transform.Rotate(Vector3.up, pInput.JRight * horizontalSpped * Time.fixedDeltaTime);

            //������ת����������Ҫ������ת�ǶȵĻ����޷�ʹ�ø�ֵ 
            //cameraHandle.transform.Rotate(Vector3.up, pInput.JUp * -vecticalSpeed * Time.deltaTime);
            //tempEulerX =cameraHandle.transform.eulerAngles.x;

            tempEulerX -= pInput.JUp * vecticalSpeed * Time.fixedDeltaTime;

            //���ı����Լ���ŷ����localEulerAngles ������EulerAngles
            cameraHandle.transform.localEulerAngles = new Vector3(Mathf.Clamp(tempEulerX, -40, 30), 0, 0);

            model.transform.eulerAngles = tempModelEuler;
        }
        else
        {
            var tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform);
        }
        //ʹ��SmoothDampȥ�Ż�׷��
        //camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position,0.2f);
        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, transform.position, ref cameraDampVelocity, 0.05f);
        mainCamera.transform.eulerAngles = transform.eulerAngles;

        mainCamera.transform.LookAt(cameraHandle.transform);
    }
    private void Update()
    {
        if (lockTarget.obj != null)
        {
            lockDot.rectTransform.position= Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position+new Vector3(0,lockTarget.halfHeight,0));
            if (Vector3.SqrMagnitude(model.transform.position - lockTarget.obj.transform.position) > 36f)
            {
                lockTarget.obj = null;
                lockDot.enabled= false;
                lockState = false;
            }
        }
    }
    public void LockUnLock()
    {
        //�˿�ģ��λ��
        var modelOrigin1 = model.transform.position;
        var modelOrigin2 = modelOrigin1 + Vector3.up;//��ǰλ�ô��      
        var boxCenter = modelOrigin2 + model.transform.forward * 5.0f;
        // ��ǰ10f�ĵ���  ���ҷ��������ģ�͵������� �����ȡ���ص��������������
        var coliders = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f), model.transform.rotation, LayerMask.GetMask("Enemy"));
        if (coliders.Length == 0)
        {
            lockTarget.obj= null;
            lockDot.enabled = false;
            lockState = false;
        }
        else
        {

            if (lockTarget.obj!=null  && lockTarget.obj == coliders[0].gameObject)
            {
                lockTarget.obj = null;
                lockDot.enabled = false;
                lockState = false;
            }
            else
            {
                lockTarget.obj= coliders[0].gameObject;
                lockTarget.halfHeight = coliders[0].bounds.extents.y;
                lockDot.enabled = true;
                lockState = true;
            }
        }
    }
    private class LockTarget
    {
        public GameObject obj;
        public float halfHeight;

        //public LockTarget(GameObject obj)
        //{
        //    this.obj = obj;
        //}
    }
}
