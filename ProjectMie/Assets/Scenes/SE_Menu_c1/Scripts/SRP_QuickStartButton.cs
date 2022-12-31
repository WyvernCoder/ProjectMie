using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_QuickStartButton : MonoBehaviour
{
    [Header("岛屿位置")][Tooltip("该选项一般交由地图管理器赋值。")]
    public Vector2 islandPos;
    public Color disableColor;

    private Image IMAGE;

    [HideInInspector] public SRP_MapManager Master;
    [HideInInspector] public int islandIndex = -1;

    private bool isSelected = false;//距离岛屿位置较近时近自动高亮(选择)该岛屿对应的按钮

    void Awake()
    {
        IMAGE = GetComponent<Image>();
    }

    void FixedUpdate()
    {
        //判断相机与本岛的距离
        if(Math_GetVectorLength(Math_Vector3to2(Camera.main.transform.position) - islandPos) <= 5f) 
        {
            isSelected = true;
            IMAGE.color = Color.white;
        }
        else 
        {
            isSelected = false;
            IMAGE.color = disableColor;
        }
    }


    public void CONTROL_Go()
    {
        if(isSelected == true) return;
        if(Master == null) Master = GameObject.Find("MapManager").GetComponent<SRP_MapManager>();
        Master.GOLocation = islandPos;
        Master.EnableGO = true;
    }



    float Math_GetVectorLength(Vector2 VECTOR, bool abs = false)
    {
        if(abs == true) return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2)));
        else return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2));
    }
    Vector2 Math_Vector3to2(Vector3 A)
    {
        return new Vector2(A.x, A.y);
    }
}
