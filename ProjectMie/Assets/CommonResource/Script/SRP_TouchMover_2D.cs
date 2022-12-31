using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_TouchMover_2D : MonoBehaviour
{
    [HideInInspector] public Vector2 MoveDirection;
    [HideInInspector] public float MoveLength;
    [Tooltip("设置要移动的相机，留空为自动搜索。")] public Camera MovingCamera;
    public float Acceleration = 0.001f;//加速度
    public float Deceleration = 0.1f;//减速度
    public bool reverseMoveDir = false;//是否反向运动
    [HideInInspector] public bool Enable = true;//控制启用禁用TouchMover
    private Vector2 MouseOriginPos;//按住左键后鼠标的原始位置
    private Vector2 MouseCurPos;//按住左键后鼠标的当前位置
    
    void Awake()
    {
        //*如果没有赋值相机，就自动寻找可用相机
        if(MovingCamera == null) 
        {
            if(Camera.main != null) MovingCamera = Camera.main;
            else if(GameObject.Find("Main Camera") != null)
            {
                MovingCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            }
            else
            {
                print("无可用相机，我亡了！");
                Destroy(gameObject);
            }
        }
    }
    
    void Update()
    {
        //左键按下，开始记录当前位置并计算出原位与现位之间的向量并解析成方向和长度
        if(Input.GetMouseButtonDown(0))
        {
            if(Enable == true)
                MouseOriginPos = Input.mousePosition;//更新鼠标的原始位置
        }

        if(Input.GetMouseButton(0))
        {
            if(Enable == true)
            {
                //更新鼠标的当前位置
                MouseCurPos = Input.mousePosition;

                //更新移动方向
                if (reverseMoveDir == true) MoveDirection = Vector3.Normalize(MouseOriginPos - MouseCurPos);
                else MoveDirection = Vector3.Normalize(MouseCurPos - MouseOriginPos);
            }
        }

        MoveLength = Math_GetVectorLength(MouseCurPos - MouseOriginPos);//更新移动距离
    }

    void FixedUpdate()
    {
        //移动相机
        MovingCamera.transform.position = Math_LerpVector(MovingCamera.transform.position, MovingCamera.transform.position + (Vector3)(MoveDirection * MoveLength), Acceleration);

        //消耗
        MouseOriginPos = Math_LerpVector(MouseOriginPos, MouseCurPos, Deceleration);
    }

    /// <summary>
    /// 初始化TouchMover，即消除一切运动因素，让速度归零
    /// </summary>
    public void InitialTouchMover()
    {
        MouseOriginPos = MouseCurPos;
        MoveLength = 0f;
    }













    float Math_GetVectorLength(Vector2 VECTOR)
    {
        return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2));
    }
    float Math_GetVectorLength(Vector3 VECTOR, bool useVec2 = false)
    {
        if(useVec2 == false) return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2) + Mathf.Pow(VECTOR.z, 2));
        else return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2));
    }

    Vector3 Math_LerpVector(Vector3 A, Vector3 B, float C)
    {
        return new Vector3(Mathf.Lerp(A.x, B.x, C), Mathf.Lerp(A.y, B.y, C), Mathf.Lerp(A.z, B.z, C));
    }
    Vector2 Math_LerpVector(Vector2 A, Vector2 B, float C)
    {
        return new Vector3(Mathf.Lerp(A.x, B.x, C), Mathf.Lerp(A.y, B.y, C));
    }
}
