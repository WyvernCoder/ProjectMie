using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_SpaceManager : MonoBehaviour
{
    [Header("对象赋值部分")]

    private GameObject LevelManager;
    public GameObject DLCContent;
    void Awake()
    {
        StartCoroutine(EnableTheMachine());
    }

    IEnumerator EnableTheMachine()
    {
        //寻找LevelManager
        while(LevelManager == null)
        {
            LevelManager = GameObject.Find("LevelManager").gameObject;
            yield return null;
        }
        StartCoroutine(Refresh());
        yield break;
    }

    public void RefreshContent() => StartCoroutine(Refresh());
    IEnumerator Refresh()
    {
        GameObject GOi;
        foreach(var GO in LevelManager.GetComponent<SRP_Mainmenu_P_Manager>().DLCPrefabList)
        {
            GOi = Instantiate(GO);

            //过滤已安装
            if(!GOi.GetComponent<SRP_DLC>().isDLCInstalled)
            {
                Destroy(GOi);
                continue;
            }

            //过滤更新补丁
            if(GOi.GetComponent<SRP_DLC>().isUpdateBundle)
            {
                Destroy(GOi);
                continue;
            }

            GOi.GetComponent<SRP_DLC>().isUnistallMode = true;//启动控件的卸载模式
            GOi.transform.SetParent(DLCContent.transform);
        }
        yield break;
    }

    public void CONTROL_CloseSpaceWindow()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
    }
}
