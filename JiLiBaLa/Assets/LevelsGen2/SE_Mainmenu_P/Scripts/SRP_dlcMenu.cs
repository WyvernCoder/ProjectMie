using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SRP_dlcMenu : MonoBehaviour
{
    void Start()
    {
        //启动游戏检测DLC列表更新
        if (SRP_DatabaseManager.Instance.bServerConnection)
        {
            //StartCoroutine(StartUpdate());自动更新已遗弃
        }
        else
        {
            print("已断网，无法获取在线内容！");
        }
    }


    // IEnumerator StartUpdate()
    // {
    //     UnityWebRequest WEB = UnityWebRequestAssetBundle.GetAssetBundle(SRP_DatabaseManager.Instance.sServerMainURL + "OnlineAssets/update.ab", (uint)SRP_DatabaseManager.Instance.iNetworkVersion,0);
    //     WEB.timeout = 0;//无法控制timeout后的情况，避免使用timeout
    //     WEB.SendWebRequest();
    //     while (!WEB.isDone)
    //     {
    //         yield return null;
    //     }
    //     print("AB更新包下载或载入完成。");
    //     AssetBundle BUNDLE = DownloadHandlerAssetBundle.GetContent(WEB);
        
    //     var prefab = BUNDLE.LoadAsset<GameObject>("PRB_DLCUpdater");
    //     prefab.GetComponent<SRP_DLCUpdater>().ABVersion = SRP_DatabaseManager.Instance.iNetworkVersion;
    //     Instantiate(prefab);
    //     //Instantiate是需要时间的
    //     BUNDLE.Unload(false);//移除AssetBundle但不移除实例化的对象，Resources.UnloadUnusedAssets
    // }
}