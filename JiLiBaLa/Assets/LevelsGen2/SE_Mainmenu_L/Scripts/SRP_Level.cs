using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SRP_Level : MonoBehaviour
{
    [HideInInspector]
    public SRP_SubMainmenu SubMainmenu;
    [HideInInspector]
    public SRP_Island MyIsland;
    [HideInInspector]
    public GameObject WinPrefab;
    [HideInInspector]
    public GameObject FailedPrefab;
    [HideInInspector]
    public int LevelIndex = -1;
    [HideInInspector]
    public GameObject Prefab = null;

    private GameObject IslandCollection;

    void Start()
    {
        IslandCollection = GameObject.Find("IslandCollection").gameObject;
    }



    public void CONTROL_LoadScene()
    {
            //!一个关卡资源包里
            //!必须项：PRB_Island作为岛屿
            //!Level需要指定Prefab作为关卡
        StartCoroutine(LoadGame());
    }
    IEnumerator LoadGame()//!读取关卡
    {
        // if(!Application.CanStreamedLevelBeLoaded("SE_Network"))
        // {
        //     print("关卡不存在，无法继续！");
        //     yield break;
        // }
        
        var LGO = GameManager.Instance.DoLoading();
        yield return new WaitForSeconds(1f);//1秒载入动画
        GameManager.Instance.MissionWinPrefab = WinPrefab;
        GameManager.Instance.MissionFaliedPrefab = FailedPrefab;
        GameManager.Instance.CurrentPrefabAsset = Prefab;
        MyIsland.GetComponent<SRP_Island>().MainmenuController.GetComponent<Canvas>().enabled = false;//隐藏左上角控制器
        SubMainmenu.enabled = false;//隐藏选关菜单
        IslandCollection.SetActive(false);//隐藏岛屿列表
        IslandCollection.GetComponent<SRP_IslandCollection>().TouchMoverPrefab.SetActive(false);
        Destroy(LGO);
        var PRB = Instantiate((GameObject)Prefab);
        GameManager.Instance.CurrentPrefab = PRB;
        PRB.transform.position = new Vector3(PRB.transform.position.x,PRB.transform.position.y, PRB.transform.position.z-1f);
        //S.allowSceneActivation = true;
        yield break;
    }
}
