using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SRP_MissionFailed : MonoBehaviour
{
    /// <summary>
    /// SRP_MissionFailed
    /// 作用：方便处理任务失败相关的事情。
    /// 用法：实例化该Prefab就可以进入任务失败动画，需要自己处理游戏暂停的事情。
    /// </summary>
    
    /// <summary>
    /// 离开
    /// </summary>
    public void CONTROLLL_LEAVE()
    {
        GameManager.Instance.PlayButtonSound();
        GameManager.Instance.BackToMainmenu();
        Destroy(gameObject,1f);//!1秒动画时间后移除自己，可能需要根据具体情况修改
    }

    /// <summary>
    /// 重新开始
    /// </summary>
    public void CONTROLLL_RESTART()
    {
        GameManager.Instance.PlayButtonSound();
        StartCoroutine(RestartMap_IE());
    }
    IEnumerator RestartMap_IE()
    {
        var LGO = GameManager.Instance.DoLoading();
        yield return new WaitForSeconds(1f);//1秒载入动画
        Destroy(LGO);
        var PRB = Instantiate(GameManager.Instance.CurrentPrefabAsset);
        PRB.transform.position = new Vector3(PRB.transform.position.x,PRB.transform.position.y, PRB.transform.position.z-1f);
        var cache = GameManager.Instance.CurrentPrefab;
        GameManager.Instance.CurrentPrefab = PRB;
        Destroy(cache);
        Destroy(gameObject);
        yield break;
    }
}
