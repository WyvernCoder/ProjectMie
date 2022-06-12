using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_StarSystem : MonoBehaviour
{
    [Header("建议有几个Sublevel就设几个星星")]
    [Header("星星数量")]
    public int StarAmount = 4;

    [Header("星星Prefab")]
    public GameObject PRB_Star;
    
    [HideInInspector]
    public int iSingleLessonIndex = -2;//这个是由SRP_SingleLesson进行赋值，-2是SingleLesson那边的初始值，用于判断是否成功初始化

    public void InitalStarSystem()
    {
        if(iSingleLessonIndex == -2)
        {
            print("未成功初始化StarSystem！");
            return;
        }

        for(int i = 0; i < StarAmount; i++)
        {
            var GO = Instantiate(PRB_Star);
            GO.transform.SetParent(transform.Find("starboard/").transform);
            GO.GetComponent<SRP_Star>().iSingleLessonIndex = iSingleLessonIndex;
            GO.GetComponent<SRP_Star>().iSublevelIndex = i;
            GO.GetComponent<SRP_Star>().InitalStar();
        }
    }
}
