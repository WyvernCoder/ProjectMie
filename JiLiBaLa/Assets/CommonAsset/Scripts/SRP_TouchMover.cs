using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_ScreenMover : MonoBehaviour
{
    [Header("对象赋值部分")]
    [Tooltip("需要移动的相机GameObject")]
    public Camera CameraNeedToMove;

    [Header("用户赋值部分")]
    [Tooltip("加速度系数。")]
    public float SpeedUpFactor = 1f;

    [Tooltip("减速度系数。")]
    public float SpeedDownFactor = 0.2f;

    [HideInInspector]
    public float MaxDistance = 200;//可向右移动的范围

    private Vector3 TouchX;//横向触控位置

    [HideInInspector]
    public float MoveLength;//鼠标点击处与鼠标当前位置的距离

    [HideInInspector]
    public bool CanMoveCamera = true;
    
    private Vector3 FinalLocation;//最终位置，相机z默认为-10


    void Awake()
    {
        FinalLocation = CameraNeedToMove.transform.position;//初始化相机位置
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && CanMoveCamera)
        {
            TouchX = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && CanMoveCamera)
        {
            if(TouchX.x > Input.mousePosition.x) MoveLength = fGetVector3Length(TouchX-Input.mousePosition);
            else MoveLength = fGetVector3Length(TouchX - Input.mousePosition) * -1f;
        }
    }

    float fGetVector3Length(Vector3 A)
    {
        return Mathf.Sqrt(Mathf.Pow(A.x, 2f) + Mathf.Pow(A.y, 2) + Mathf.Pow(A.z, 2));
    }
    
    void FixedUpdate()//使移动不随帧数影响
    {
        if (CameraNeedToMove.transform.position.x > MaxDistance)//限制措施
        {
            CameraNeedToMove.transform.position -= new Vector3(0.01f, 0, 0);
            FinalLocation = CameraNeedToMove.transform.position;
            MoveLength = 0;
        }
        if (CameraNeedToMove.transform.position.x < 0)//限制措施
        {
            CameraNeedToMove.transform.position += new Vector3(0.01f, 0, 0);
            FinalLocation = CameraNeedToMove.transform.position;
            MoveLength = 0;
        }
        if (Input.GetMouseButton(0)) TouchX = Vector3.Lerp(TouchX,Input.mousePosition,0.1f);
        CameraNeedToMove.transform.position = Vector3.Lerp(CameraNeedToMove.transform.position, FinalLocation, 0.1f*SpeedUpFactor);
        FinalLocation = new Vector3(FinalLocation.x + MoveLength*0.001f,FinalLocation.y,FinalLocation.z);
        if (CanMoveCamera) MoveLength = Mathf.Lerp(MoveLength, 0f, 0.05f * SpeedDownFactor);
        else MoveLength = 0;
        MaxDistance = 200f;
    }
}
