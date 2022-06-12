using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_FenceGenerator : MonoBehaviour
{
    public GameObject FenceTempleGO;    //需要生成的模板

    [HideInInspector]
    public List<GameObject> FenceList = new List<GameObject>();

    public float MaxLength = 200f;    //最大距离，超过此距离将不再生成

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
        FenceList.Add(FenceTempleGO);
        while (transform.TransformPoint(FenceList[FenceList.Count - 1].gameObject.transform.position).x <= MaxLength)
        {
            GOCache = Instantiate(FenceTempleGO);
            GOCache.transform.position = new Vector3(transform.TransformPoint(FenceList[FenceList.Count - 1].gameObject.transform.Find("RightAnchor/").transform.position).x + 9.6f, GOCache.transform.TransformPoint(transform.position).y, -1f);
            FenceList.Add(GOCache);
        }
    }
}
