using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_Bird : MonoBehaviour
{
    private Vector2 LastPosition = new Vector2(0,0);
    private Vector2 Velocity;
    private Vector2 SweepLocation;//飞鸟会慢慢移动到这个位置
    public Vector2 SweepSpeedRange = new Vector2(0.06f, 0.2f);
    private float SweepSpeed;
    public float RotateSpeed = 0.01f;
    public float Radiuss = 7f;
    public bool ControlWithMouse = false;//鸟是否绕着鼠标飞行，
    private float randomRotate;//角度增量，用于计算圆周路径
    private Vector2 CircleHeartPos;

    public AudioClip AC_Kill;
    public AudioClip AC_Die1;
    public AudioClip AC_Die2;
    public Transform RootTransform;

    private AudioSource AS;
    private SRP_TouchMover_2D TouchMover;
    private bool SetToMouseLocation = false;//鼠标“强制”控制鸟的位置

    void Awake()
    {
        randomRotate = UnityEngine.Random.Range(0f, 360f);
        SweepSpeed = UnityEngine.Random.Range(SweepSpeedRange.x, SweepSpeedRange.y);
        if(GameObject.Find("PRB_TouchMover_2D") != null)
            TouchMover = GameObject.Find("PRB_TouchMover_2D").GetComponent<SRP_TouchMover_2D>();
        AS = GetComponent<AudioSource>();
        CircleHeartPos = Math_Vector3to2(RootTransform.position);
        Radiuss += UnityEngine.Random.Range(-0.5f, 0.6f);
    }

    void FixedUpdate()
    {
        //计算鸟的速度向量
        Velocity = ((Vector2)transform.position - LastPosition) * 100f;
        LastPosition = transform.position;

        //更新鸟的位置
        if(SetToMouseLocation == false) //切换鼠标控制鸟
            transform.position = Math_MoveVector(transform.position, Math_LerpVector(transform.position, Math_Vector2to3(SweepLocation), SweepSpeed), SweepSpeed);
        else transform.position = GetMousePosition();

        //更新增量，用于控制圆形轨道的角度变化量
        randomRotate += 0.5f;

        //更新移动位置
        SweepLocation = Math_Path_Circle(Selector(GetMousePosition(), Math_Vector2to3(CircleHeartPos), ControlWithMouse), Radiuss, randomRotate);
    }

    void Update()
    {
        //更新鸟的方向
        if(Math_GetVectorLength(Velocity, true) > 2f)
        {
            //transform.rotation = Quaternion.Euler(Math_LerpVector(transform.rotation.eulerAngles, new Vector3(0f,0f,Math_GetRotationBetweenVector_360(Velocity, new Vector2(1, 0))), RotateSpeed));
            float newRotation = Math_GetRotationBetweenVector_360(Velocity, new Vector2(1, 0));
            transform.rotation = Quaternion.Euler(Selector(new Vector3(0f, 0f, newRotation), Math_LerpVector(transform.rotation.eulerAngles, new Vector3(0f, 0f, newRotation), RotateSpeed), Mathf.Abs(newRotation) < 1f ));
        }
    }

    int clickCount = 0;
    public void CONTROL_MouseDown()
    {
        if(TouchMover != null)
            TouchMover.Enable = false;
        SetToMouseLocation = true;
        clickCount++;
        if(clickCount == 2) 
        {
            SweepSpeed = 0;
            AS.PlayOneShot(AC_Kill);
            int random = UnityEngine.Random.Range(0,2);
            switch(random)
            {
                case 0: AS.PlayOneShot(AC_Die1); break;
                case 1: AS.PlayOneShot(AC_Die2); break;
            }
        }
    }

    public void CONTROL_MouseUp()
    {
        if(TouchMover != null)
            TouchMover.Enable = true;
        SetToMouseLocation = false;
        Invoke("ConsumeClickCount", 0.3f);
    }

    void ConsumeClickCount()
    {
        clickCount--;
        //print("消耗一次！" + clickCount);
        if(clickCount < 0) clickCount = 0;
    }



    Vector3 GetMousePosition()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(pos.x, pos.y, 0f);
    }





    Vector2 Math_Path_Circle(Vector2 Origin, float Radius, float Degree)
    {   //角度是以(1, 0)方向为0度的
        Degree = Degree % 360f;//使角度始终限制在360度内
        return new Vector2(Radius * Mathf.Cos(Degree * (Mathf.PI / 180f)), Radius * Mathf.Sin(Degree * (Mathf.PI / 180f))) + Origin;
    }

    float Math_GetRotationBetweenVector_360(Vector2 A, Vector2 B)//能够将0~180度映射至0~360度的版本
    {   //叉乘<0时朝上，叉乘>0时朝下
        if(Math_Cross(Math_Normalize(Math_Vector2to3(A)), Math_Normalize(Math_Vector2to3(B))).z > 0) 
        {
            return 360 - Math_GetRotationBetweenVector(A, B);
        }
        else return Math_GetRotationBetweenVector(A, B);
    }
    float Math_GetRotationBetweenVector(Vector2 A, Vector2 B)//Acos只能输出0~180度
    {   //Acos返回的是弧度，后面乘数转成角度
        return (float)Mathf.Acos(Mathf.Clamp(Math_Dot(Math_Normalize(A), Math_Normalize(B)), -1f, 1f)) * (180f / Mathf.PI);
    }
    float Math_GetVectorLength(Vector2 VECTOR, bool abs = false)
    {
        if(abs == true) return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2)));
        else return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2));
    }
    float Math_GetVectorLength(Vector3 VECTOR, bool useVec2 = false, bool abs = false)
    {
        if (useVec2 == false)
        {
            if(abs == true) return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2) + Mathf.Pow(VECTOR.z, 2)));
            else return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2) + Mathf.Pow(VECTOR.z, 2));
        }
        else
        {
            if(abs == true) return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2)));
            else return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2));
        }
    }
    float Math_Dot(Vector2 A, Vector2 B)
    {
        return A.x * B.x + A.y * B.y;
    }
    float Math_Cross(Vector2 A, Vector2 B)
    {
        return (A.x * B.y - A.y * B.x);
    }
    Vector3 Math_Cross(Vector3 A, Vector3 B)
    {
        return new Vector3(A.y * B.z - A.z * B.y, A.z * B.x - B.z * A.x, A.x * B.y - A.y * B.x);
    }
    Vector3 Math_Vector2to3(Vector2 A)
    {
        return new Vector3(A.x, A.y, 0f);
    }
    Vector2 Math_Vector3to2(Vector3 A)
    {
        return new Vector2(A.x, A.y);
    }
    Vector3 Math_Normalize(Vector3 A)
    {
        if(A.x == 0) A.x = Mathf.Epsilon;
        if(A.y == 0) A.y = Mathf.Epsilon;
        if(A.z == 0) A.z = Mathf.Epsilon;
        return new Vector3(A.x / Math_GetVectorLength(A, false, true), A.y / Math_GetVectorLength(A, false, true), A.z / Math_GetVectorLength(A, false, true));
    }
    Vector2 Math_Normalize(Vector2 A)
    {
        if(A.x == 0) A.x = Mathf.Epsilon;
        if(A.y == 0) A.y = Mathf.Epsilon;
        return new Vector2(A.x / Math_GetVectorLength(A, true), A.y / Math_GetVectorLength(A, true));
    }
    Vector3 Math_LerpVector(Vector3 A, Vector3 B, float C)
    {
        return new Vector3(Mathf.Lerp(A.x, B.x, C), Mathf.Lerp(A.y, B.y, C), Mathf.Lerp(A.z, B.z, C));
    }
    Vector2 Math_LerpVector(Vector2 A, Vector2 B, float C)
    {
        return new Vector3(Mathf.Lerp(A.x, B.x, C), Mathf.Lerp(A.y, B.y, C));
    }
    Vector3 Math_MoveVector(Vector3 A, Vector3 B, float C)
    {
        return new Vector3(Mathf.MoveTowards(A.x, B.x, C), Mathf.MoveTowards(A.y, B.y, C), Mathf.MoveTowards(A.z, B.z, C));
    }
    Vector2 Math_MoveVector(Vector2 A, Vector2 B, float C)
    {
        return new Vector2(Mathf.MoveTowards(A.x, B.x, C), Mathf.MoveTowards(A.y, B.y, C));
    }

    public T Selector<T>(T A, T B, bool isA)
    {
        if(isA) return A;
        else return B;
    }
}
