using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SRP_IslandCollection : MonoBehaviour
{
    [Header("对象赋值部分")]

    [Tooltip("触摸移动功能Prefab，用于设置其可否继续移动和最远移动距离。")]
    public GameObject TouchMoverPrefab;
    [HideInInspector]
    public SRP_TouchMover _TouchMoverPrefab;

    [HideInInspector]
    public Canvas MainmenuControlCanvas;

    public GameObject IslandCollection;

    [Header("用户赋值部分")]

    [Tooltip("在每个Island之间插入空白距离。")]
    public float SpawnOffset = 0f;

    [Tooltip("禁用岛屿自解压功能，测试使用。")]
    public bool DisableAutospawn = false;

    void Awake()
    {
        _TouchMoverPrefab = TouchMoverPrefab.GetComponent<SRP_TouchMover>();
        StartCoroutine(Start_IE());
    }

    void Start()
    {
        MainmenuControlCanvas = GameObject.Find("Canvas_Control").GetComponent<Canvas>();
        GameManager.Instance.IslandCollection = gameObject;
        if(!Application.isEditor && DisableAutospawn) print("你可能忘记在IslandCollection中启动自动生成Island了。");
    }

    IEnumerator Start_IE()
    {
        if(DisableAutospawn) yield break;
        var SpawnLocation = Vector3.zero;
        Transform GOTransform;
        string[] filedir = Directory.GetFiles(Application.persistentDataPath + "/DownloadContent");
        foreach(var S in filedir)//!S是FullPath
        {
            var ABR = AssetBundle.LoadFromFileAsync(S);
            while(!ABR.isDone) yield return null;
            var ABR2 = ABR.assetBundle.LoadAssetAsync("PRB_Island");//!固定读取PRB_Island这个名称
            while(!ABR2.isDone) yield return null;
            var GO = ABR2.asset;
            if(GO == null)
            {
                ABR.assetBundle.Unload(false);
                continue;
            }
            GOTransform = Instantiate<GameObject>(GO as GameObject).transform;//!GOTransform是Island
            GOTransform.SetParent(IslandCollection.transform, true);
            GOTransform.localPosition = SpawnLocation;
            GOTransform.gameObject.GetComponent<SRP_Island>().ABFullPath = S;
            //GOTransform.gameObject.GetComponent<SRP_Island>().MyLevelName = Path.GetFileNameWithoutExtension(S);
            _TouchMoverPrefab.MaxDistance = SpawnLocation.x;

            SpawnLocation = GOTransform.Find("RightAnchor/").transform.position + new Vector3(GOTransform.Find("RightAnchor/").transform.position.x - GOTransform.position.x + SpawnOffset,0f,0f);
            ABR.assetBundle.Unload(false);
            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }

    
}
