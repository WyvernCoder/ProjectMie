using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_MapManager : MonoBehaviour
{
    [Header("引用设置")]
    public SRP_TouchMover_2D TouchMover;
    public Slider MapSlider;
    public RectTransform NavButtonCanvas;//所有导航按钮都会生成在该结构下
    public SRP_QuickStartButton NavButtonPrefab;//导航按钮Prefab
    public GameObject IslandCollection;//所有小岛都应该在该结构下
    private List<GameObject> IslandList = new List<GameObject>();

    private float CameraSize = 5.4f;//相机FOV最终会插值到CameraSize


    
    [Space(10)][Header("导航设置")]
    public float GOSpeed = 0.01f;//导航速度
    private float GOFullDistance;
    [HideInInspector] public bool EnableGO = false;//是否启用导航模式
    [HideInInspector] public Vector2 GOLocation;//导航模式目的地

    [Space(10)][Header("岛屿名称列表")]
    [Header("其他设置")][Tooltip("在此处按顺序填写岛屿名")]
    public List<string> IslandNameList_Old = new List<string>();//岛屿名称集合
    public List<RectTransform> IslandButtonList = new List<RectTransform>();//岛屿按钮合集


    void Awake()
    {
        APIManager.GENERATE_BODY();

        //初始化IslandList并生成导航按钮
        int index = 0;
        foreach(Transform T in IslandCollection.transform) 
        {
            if(index >= IslandButtonList.Count) break;
            IslandButtonList[index].GetComponent<SRP_QuickStartButton>().islandIndex = index;
            IslandButtonList[index].GetComponent<SRP_QuickStartButton>().islandPos = T.position;
            index++;
        }

        UserManager.SetUserSubscribeClassBool("category01_class01",true);
    }

    void Start()
    {
        /* 无论如何，第一关卡总是要被解锁的 */
        /* 但由于加载顺序的问题，你不能把它加在这里。 */
        //
    }




    public void CONTROL_ChangeSlide()
    {
        CameraSize = Mathf.Lerp(5.4f, 20f, MapSlider.normalizedValue);
    }

    public void TouchMover_Disable()
    {
        TouchMover.Enable = false;
    }

    public void TouchMover_Enable()
    {
        TouchMover.Enable = true;
    }

    bool doOnce = false;
    void Update()
    {
        //判断是否启用导航模式
        if(EnableGO == true)
        {
            TouchMover.Enable = false;

            if(doOnce == false)
            {   //更新总路程
                doOnce = true;
                GOFullDistance = Math_GetVectorLength(GOLocation - Math_Vector3to2(Camera.main.transform.position), true);
            }

            //更新导航模式下相机的位置和FOV
            Camera.main.transform.position = Math_GradVector(Camera.main.transform.position, Math_Vector2to3(GOLocation) + new Vector3(0, 0, Camera.main.transform.position.z), GOSpeed * Time.deltaTime, GOSpeed * 0.5f * Time.deltaTime);
            MapSlider.value = Mathf.Lerp(MapSlider.value, Mathf.Clamp(1.5f * Mathf.Sin(    Math_GetVectorLength(GOLocation - Math_Vector3to2(Camera.main.transform.position), true)    * Mathf.PI  /   GOFullDistance   ), 0f, 1f), GOSpeed * 0.5f * Time.deltaTime);

            if(Math_GetVectorLength(GOLocation - Math_Vector3to2(Camera.main.transform.position), true) < 1f)
            {
                EnableGO = false;
                doOnce = false;
                MapSlider.value = 0f;
                TouchMover.InitialTouchMover();//防止镜头拉回原位
                TouchMover.Enable = true;
                Camera.main.transform.position = Math_Vector2to3(GOLocation) + new Vector3(0, 0, Camera.main.transform.position.z);
            }
        }

        //更新相机FOV
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, CameraSize, 10f * Time.deltaTime);
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
    Vector3 Math_GradVector(Vector3 A, Vector3 B, float MoveSpeed, float LerpSpeed)
    {
        return Math_MoveVector(A, Math_LerpVector(A, B, LerpSpeed), MoveSpeed);
    }
    Vector2 Math_GradVector(Vector2 A, Vector2 B, float MoveSpeed, float LerpSpeed)
    {
        return Math_MoveVector(A, Math_LerpVector(A, B, LerpSpeed), MoveSpeed);
    }

    public T Selector<T>(T A, T B, bool isA)
    {
        if(isA) return A;
        else return B;
    }
}
