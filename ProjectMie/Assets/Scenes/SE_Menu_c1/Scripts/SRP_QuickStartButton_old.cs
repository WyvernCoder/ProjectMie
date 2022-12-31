using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_QuickStartButton_old : MonoBehaviour
{
    [Header("岛屿位置")][Tooltip("该选项一般交由地图管理器赋值。")]
    public Vector2 islandPos;

    public Sprite IMG_Selected;
    public Sprite IMG_Unselected;
    public TMPro.TMP_Text TEXT;

    [HideInInspector] public SRP_MapManager Master;
    [HideInInspector] public int islandIndex = -1;

    private UnityEngine.UI.Image image;
    private bool isSelected = false;//距离岛屿位置较近时近自动高亮(选择)该岛屿对应的按钮

    void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
    }

    void FixedUpdate()
    {
        //判断相机与本岛的距离
        if(Math_GetVectorLength(Math_Vector3to2(Camera.main.transform.position) - islandPos) <= 5f) 
        {
            isSelected = true;
            image.sprite = IMG_Selected;
            TEXT.color = Color.gray;
        }
        else 
        {
            isSelected = false;
            image.sprite = IMG_Unselected;
            TEXT.color = Color.white;
        }
    }


    public void CONTROL_Go()
    {
        if(isSelected == true) return;
        if(Master == null) Master = GameObject.Find("MapManager").GetComponent<SRP_MapManager>();
        Master.GOLocation = islandPos;
        Master.EnableGO = true;
    }

    public void SetIslandName(string name)
    {
        TEXT.text = name;
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
