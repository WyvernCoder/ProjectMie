using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SRP_Mainmenu_P_Manager : MonoBehaviour
{
    
    [Header("对象赋值部分")]
    [Tooltip("游戏菜单Canvas。")]
    public Canvas GameMenuCanvas;
    [Tooltip("DLC菜单Canvas。")]
    public Canvas dlcMenuCanvas;
    [Tooltip("我的菜单Canvas。")]
    public Canvas PrivateCanvas;
    [Tooltip("空间管理Canvas。")]
    public Canvas SpaceCanvas;

    [HideInInspector]
    public List<GameObject> DLCPrefabList = new List<GameObject>();

    [HideInInspector]
    public GameObject InstallWindowPrefab;
    [Tooltip("安装窗口Prefab")]
    public GameObject InstallWindowPrefab_;



    [Tooltip("ScrollView的Content GameObject。")]
    public GameObject ScrollViewContent;

    [Tooltip("用于序列化的DLCPrefab。")]
    public GameObject DLCPrefabToSerializable;

    [Tooltip("DLC列表")]
    [Header("不要添加任何资源。")]
    public List<DLCFormat> DLCList = new List<DLCFormat>();

    [System.Serializable]
    public struct DLCFormat
    {
        [Tooltip("DLC标题名，建议填写。")]
        public string DLCName;
        [Tooltip("关卡名，必填。")]
        public string SceneName;
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
    
    void Start()
    {
        InstallWindowPrefab = Instantiate(InstallWindowPrefab_);
        InstallWindowPrefab.GetComponent<Canvas>().sortingOrder = -1000;

        //初始化DLC目录
        RefreshDLCContent(true);
        UnpackLocalUpdate();

        //显示安卓状态栏
        // GameManager.statusBarState = GameManager.States.TranslucentOverContent;//透明风格
        // GameManager.AndroidStatusBar();

        //初始化菜单
        //dlcMenuCanvas.enabled = false; //!需要等它添加完ScrollView元素再隐藏掉！具体看协程
        GameMenuCanvas.enabled = true;
        PrivateCanvas.enabled = false;
        SpaceCanvas.enabled = false;
    }

    public void UnpackLocalUpdate() => StartCoroutine(_DoUpdate());//解包本地更新补丁
    IEnumerator _DoUpdate()
    {
        if(!File.Exists(Application.persistentDataPath + "/DownloadContent/" + "update" + ".ab"))
        {
            print("寄了，没有找到update.ab，你可能没有更新过你的DLC列表。");
            yield break;
        }
        AssetBundleCreateRequest AB = AssetBundle.LoadFromFileAsync(Application.persistentDataPath + "/DownloadContent/" + "update" + ".ab");
        while(!AB.isDone) yield return null;
        if(AB.assetBundle == null)
        {
            print("更新文件是空文件！");
            yield break;
        }
        var AB2 = AB.assetBundle.LoadAllAssetsAsync();
        while(!AB2.isDone) yield return null;
        foreach(var GO in AB2.allAssets) Instantiate(GO);
        yield return new WaitForSeconds(0.2f);
        AB.assetBundle.Unload(false);
        print("本地更新补丁更新成功。");
        yield break;
    }

    public void RefreshDLCContent(bool isFirstRun = false) => StartCoroutine(RefreshDLCContent_IE(isFirstRun));
    IEnumerator RefreshDLCContent_IE(bool isFirstRun = false)
    {
        int OriginalSortOrder = 0;
        if(dlcMenuCanvas.enabled == false)//!在未enable的情况下去添加Child，安卓会崩！
        {
            dlcMenuCanvas.enabled = true;
            OriginalSortOrder = dlcMenuCanvas.sortingOrder;
            dlcMenuCanvas.sortingOrder = -1000;//隐藏掉？
            isFirstRun = true;//添加完东西后再关闭菜单
        }
        //刷新DLC目录
        foreach(Transform T in ScrollViewContent.transform)
        {
            Destroy(T.gameObject);
        }
        DLCPrefabList.Clear();
        yield return null;
        
        // foreach(var GO in DLCPrefabList)
        // {
        //     Instantiate(GO).transform.SetParent(ScrollViewContent.transform, true);
        // }
        // foreach(var GO in DLCPrefabList)
        // {
        //     Destroy(GO);
        // }

        foreach (var S in DLCList)
        {   //!顶部用于更新的DLC
            SRP_DLC GO = Instantiate(DLCPrefabToSerializable).GetComponent<SRP_DLC>();
            GO.transform.SetParent(ScrollViewContent.transform, true);
            DLCPrefabList.Add(GO.gameObject);
            GO.InitalThisNewGen(this,S.SceneName,S.InstallDescribe,S.UnistallDescribe,S.DLCName);
            GO.InitalThis(S.InstalledImage, S.InstallingImage, S.UninstallImage, S.NotInstallImage, S.InstallDescribe, S.SetAsUpdateBundle);
            if(S.SetAsUpdateBundle) GO.TitleText.GetComponent<TMP_Text>().text = "更新使用者界面";
        }
        yield return null;
        if(isFirstRun) 
        {
            dlcMenuCanvas.sortingOrder = OriginalSortOrder;//恢复显示顺序
            dlcMenuCanvas.enabled = false;//隐藏掉
        }
        yield break;
    }
}
