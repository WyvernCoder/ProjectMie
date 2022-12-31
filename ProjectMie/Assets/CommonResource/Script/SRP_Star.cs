using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_Star : MonoBehaviour
{
    [Header("空星星的图片")]
    [Header("请确保两张图片像素完全相同")]
    public Sprite EmptyStar;
    [Header("实星星的图片")]
    public Sprite FullStar;

    [HideInInspector]
    public int iSingleLessonIndex = -1;
    [HideInInspector]
    public int iSublevelIndex = -1;

    void Start()
    {
        
    }

    public void InitalStar()
    {
        if(iSingleLessonIndex == -1 || iSublevelIndex == -1)
        {
            print("星星未被正确初始化！");
            return;
        }
    }
}
