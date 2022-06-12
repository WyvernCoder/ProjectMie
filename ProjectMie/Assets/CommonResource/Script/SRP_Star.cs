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
        if(APIManager.API == null || NetManager.NET == null)
        {
            //print("星星死掉了！");
            return;
        }

        if(NetManager.NET.IsLogin() == false)
        {
            //print("玩家没有登录！");
            return;
        }
    }

    public void InitalStar()
    {
        if(iSingleLessonIndex == -1 || iSublevelIndex == -1)
        {
            print("星星未被正确初始化！");
            return;
        }
        
        //打开Sublevel目录之前，数据库里是没有Star的分数数据的
        //这条是看看是否有分数数据，如果没有分数数据，说明还没有玩，当然是0分没有星星，什么都不干，保持默认空星星就可以了
        if(NetManager.NET.GetUserData().ClassData[APIManager.API.API_LessonIndexCollection_Get().x].LessonList[iSingleLessonIndex].SublevelScore.Count == 0) 
            return;

        //如果该Sublevel对应的分数大于等于100，就把图片设为实体星星
        if(NetManager.NET.GetUserData().ClassData[APIManager.API.API_LessonIndexCollection_Get().x].LessonList[iSingleLessonIndex].SublevelScore[iSublevelIndex] >= 100 )
        {
            GetComponent<SpriteRenderer>().sprite = FullStar;
        }
    }
}
