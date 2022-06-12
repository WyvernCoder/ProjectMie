using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SRP_DLCUpdater : MonoBehaviour
{
    [Tooltip("该AB包的版本号")]
    public int ABVersion = 0;

    [Tooltip("DLC列表")]
    public List<DLCFormat> DLCList = new List<DLCFormat>();

    [System.Serializable]
    public struct DLCFormat
    {
        [Tooltip("DLC标题名，建议填写。")]
        public string DLCName;
        [Tooltip("该DLC对应的AB包名")]
        public string ABPackageName;
        [Tooltip("安装时的介绍。")]
        public string InstallDescribe;
        [Tooltip("卸载时的介绍。")]
        public string UnistallDescribe;
        [Tooltip("未安装图标，选填。")]
        public Sprite NotInstallImage;
        [Tooltip("卸载的图标，选填。")]
        public Sprite UninstallImage;
        [Tooltip("已安装图标，选填。")]
        public Sprite InstalledImage;
        [Tooltip("安装中图标，选填。")]
        public Sprite InstallingImage;
        [Tooltip("将该项作为更新项，选填。")]
        public bool SetAsUpdateBundle;
    }
    void Awake()
    {
        //启动update病毒
        StartCoroutine(SSTart());
    }

    IEnumerator SSTart()
    {
        //寻找宿主
        SRP_Mainmenu_P_Manager MainmenuManager = GameObject.Find("LevelManager").gameObject.GetComponent<SRP_Mainmenu_P_Manager>();

        //清空宿主的大脑
        MainmenuManager.DLCList.Clear();

        //读取病毒信息并注入
        foreach(DLCFormat S in DLCList)
        {
            var cache = new SRP_Mainmenu_P_Manager.DLCFormat();//模拟宿主信息格式
            cache.DLCName = S.DLCName;
            cache.SceneName = S.ABPackageName;
            cache.InstallDescribe = S.InstallDescribe;
            cache.UnistallDescribe = S.UnistallDescribe;
            cache.NotInstallImage = S.NotInstallImage;
            cache.UninstallImage = S.UninstallImage;
            cache.InstalledImage = S.InstalledImage;
            cache.InstallingImage = S.InstallingImage;
            cache.SetAsUpdateBundle = S.SetAsUpdateBundle;
            MainmenuManager.DLCList.Add(cache);//注入病毒
        }
        yield return null;

        //清空宿主实例化的细胞
        foreach(var GO in MainmenuManager.DLCPrefabList) Destroy(GO);
        yield return null;

        //然后宿主使用RefreshContent刷新列表
        MainmenuManager.RefreshDLCContent();

        //更新本地版本号
        PlayerPrefs.SetInt("version",ABVersion);
        
        //病毒外壳与细胞分离
        Destroy(gameObject);

        yield break;
    }
}
