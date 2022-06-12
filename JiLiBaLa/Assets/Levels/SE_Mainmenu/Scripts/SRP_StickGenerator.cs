using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_StickGenerator : MonoBehaviour
{
    public GameObject StickTempleGO;    //需要生成的模板

    [HideInInspector]
    public List<GameObject> StickList = new List<GameObject>();

    public float MaxLength = 0f;    //最大距离，超过此距离将不再生成

    void Start()
    {
        if(GameObject.Find("PRB_Train") == null || GameManager.Instance == null) 
        {
            StartGenerator();//便于调试
        }
    }

    public void StartGenerator()   //MaxLenght是基于火车厢位置的，所以必须等火车厢生成完才能生成Stick
    {
        GameObject GOCache;
        StickList.Add(StickTempleGO);
        while (transform.TransformPoint(StickList[StickList.Count - 1].gameObject.transform.position).x <= MaxLength)
        {
            GOCache = Instantiate(StickTempleGO);
            GOCache.transform.SetParent(gameObject.transform, true);
            GOCache.transform.position = new Vector3(transform.TransformPoint(StickList[StickList.Count - 1].gameObject.transform.Find("RightAnchor/").transform.position).x - (StickTempleGO.transform.Find("RightAnchor/").position.x - StickTempleGO.transform.Find("LeftAnchor/").position.x), GOCache.transform.TransformPoint(transform.position).y, 0f);
            StickList.Add(GOCache);
        }
    }
}
