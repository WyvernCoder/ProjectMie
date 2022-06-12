using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_CloudGenerator : MonoBehaviour
{
    public GameObject FenceTempleGO;    //需要生成的模板
    private GameObject CameraM;
    private GameObject GOCache;

    [HideInInspector]
    public List<GameObject> FenceList = new List<GameObject>();

    public float MaxLength = 200f;    //最大距离，超过此距离将不再生成

    void Start()
    {
        CameraM = GameObject.Find("ZoomingCamera");
        if(GameObject.Find("PRB_Train") == null || GameManager.Instance == null) 
        {
            StartGenerator();//便于调试
        }
    }

    public void StartGenerator()   //MaxLenght是基于火车厢位置的，所以必须等火车厢生成完才能生成Stick
    {
        
        FenceList.Add(FenceTempleGO);
        while (transform.TransformPoint(FenceList[FenceList.Count - 1].gameObject.transform.position).x <= MaxLength)
        {
            CreateOneCloud();
        }
    }
    public void CreateOneCloud()
    {
            FenceTempleGO.GetComponent<SRP_CloudMoveSystem>().HeadScript = gameObject;
            GOCache = Instantiate(FenceTempleGO);
            GOCache.transform.SetParent(gameObject.transform, true);
            GOCache.transform.position = new Vector3(transform.TransformPoint(FenceList[FenceList.Count - 1].gameObject.transform.Find("RightAnchor/").transform.position).x + Random.Range(-12f,12f), GOCache.transform.TransformPoint(transform.position).y, 0f);
            FenceList.Add(GOCache);
    }
}