using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SRP_TrainBody : MonoBehaviour
{    
    [Header("对象引用部分")]

    [HideInInspector]
    public GameObject TrainHeadObject;//车头GameObject
    [HideInInspector]
    public GameObject ZoomingCamera;
    [HideInInspector]
    public GameObject OriginalCamera;

    [Tooltip("默认是火车厢 Prefab 中的 AnchorRight GameObject ，用于确定下一节车厢生成的位置。")]
    public GameObject RightAnchor;
    public GameObject CenterLocation;
    public List<GameObject> WindowObjects = new List<GameObject>();

    [HideInInspector]
    public int TrainBodyIndex;//作用同等于GameManger中的iCurrentTrainBodyIndex

    void Start()
    {   
        //使用if减少Find使用次数
        if (TrainHeadObject == null) TrainHeadObject = GameObject.Find("PRB_Train");//初始化TrainHeadObject
    }







}
