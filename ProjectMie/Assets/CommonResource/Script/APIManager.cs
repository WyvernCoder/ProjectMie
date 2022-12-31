using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEditor;

public class APIManager : MonoBehaviour
{
    [HideInInspector]
    public static APIManager API;   //静态类型变量，便于访问APIManager

    [Header("Http服务器")][Tooltip("将在此服务器下载图片、视频等二进制数据。")]
    public string HttpServer = @"127.0.0.1:9999";

    [Tooltip("按钮声音列表。")]
    public List<AudioClip> ButtonSoundList = new List<AudioClip>();

    [Tooltip("包含转场动画的Prefab。")]
    public GameObject TransitionPrefab;
    private GameObject goTransitionPrefab;

    [Tooltip("包含胜利功能的Prefab。")]
    public GameObject WinPrefab;
    private GameObject goWinPrefab;

    [Tooltip("包含失败功能的Prefab。")]
    public GameObject FailPrefab;
    private GameObject goFailPrefab;

    [Tooltip("包含庆祝功能的Prefab。")]
    public GameObject CelebratePrefab;

    private string sLoadingSceneName = "NONE";  //用于记录即将要载入Scene的名字
    private AudioSource asAudioPlayer;  //用于播放按钮声音的AudioSource
    private bool bIsGameLoading = false;//记录当前是否正处于载入状态，由SRP_LoadingScene维护
    
    [HideInInspector]
    public List<string> SceneHistory = new List<string>();// 场景的历史记录，便于制作返回功能。

    void Awake()
    {
        if(API != null) Destroy(gameObject);    //去重，防止多个APIManager出现
        API = this;     //为static变量赋值
        DontDestroyOnLoad(this);
        asAudioPlayer = gameObject.AddComponent<AudioSource>(); //添加一个AudioSource，方便播放声音。
        SceneHistory.Add(SceneManager.GetActiveScene().name);
        
        /* 启动SQL服务器 */
        UserManager.SQL_Open();
    }

    void OnDestroy()
    {
        UserManager.SQL_Close();
    }

    /// <summary>
    /// 在任何时刻任何地点创建一个APIManager。
    /// 你不需要担心重复创建的问题。
    /// 并且，没人知道它为什么要叫GENERATE_BODY。
    /// </summary>
    public static void GENERATE_BODY()
    {   /* 科普：GENERATE_BODY是UE4将CPP文件录入其反射系统的宏的名称 */
        if(APIManager.API != null) return;

        /* 铁打不动的路径 */
        const string APIManagerPath = @"PrefabCore/APIManager";
        
        // AssetDatabase只能在Editor使用
        //Instantiate(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(APIManagerPath));
        Instantiate(Resources.Load<UnityEngine.Object>(APIManagerPath));

        /* 如果你不在竖屏主页面，就自动登录一个账号 */
        if(UserManager.isLogin != true && SceneManager.GetActiveScene().name != "SE_Menu_P")
        {   
            /* 众所周知，Microsoft 正想办法推广无密码登录模式 */
            if (UserManager.FindSameUser(UserInfoField.username, "debug") == 0)
                UserManager.CreateUser("debug", "", "");

            UserManager.Login("debug", "");
            if (UserManager.isLogin) Debug.Log("已自动登录 Debug 账号。");
        }
    }

    /// <summary>
    /// 载入关卡。
    /// 需使用API_LoadScene_SetName()方法设置要载入的关卡名。
    /// </summary>
    public void API_LoadScene() 
    {   
        if(sLoadingSceneName == "NONE")
        {
            Debug.LogAssertion("未设置载入地图名。");
            return;
        }

        /* 写入历史记录 */
        SceneHistory.Add(API_LoadScene_GetName());

        //开始协程
        StartCoroutine(API_LoadScene_IE());
    }
    IEnumerator API_LoadScene_IE()
    {
        API_IsGameLoading(true);
        API_Transition_Start();//开始转场
        yield return new WaitForSeconds(2f);//先开始转场，等几秒后再开始载入地图，防止加载过快造成下一关卡的声音被提前播放

        //切换到SE_Loading这个Scene，第二个参数是当Scene加载完成后，关闭其他所有的Scene，相关的还有Additive模式，后面会用到
        if (Application.CanStreamedLevelBeLoaded(API_LoadScene_GetName()) == false)
        {
            SceneManager.LoadScene("SE_Loading");
        }
        else
        {
            var load = SceneManager.LoadSceneAsync(API_LoadScene_GetName());
            while(load.isDone == false) yield return null;
            API_IsGameLoading(false);
            API_Transition_Stop();
        }

        yield break;//这个语句的作用是停止协程
    }

    /// <summary>
    /// 获取当前关卡的名或AB包名
    /// </summary>
    /// <returns>字符串</returns>
    public string API_LoadScene_GetName()
    {
        return sLoadingSceneName;
    }

    /// <summary>
    /// 设置要载入的Scene或AB包名
    /// </summary>
    /// <param name="Name"></param>
    public void API_LoadScene_SetName(string Name)
    {
        sLoadingSceneName = Name;
    }

    /// <summary>
    /// 直接返回主菜单。
    /// 该方法的横屏主菜单是指选择大类的横屏主菜单。
    /// 若你是要从游戏关卡返回到选关横屏菜单，请使用API_SceneGoBack()方法
    /// </summary>
    /// <param name="isMenuP">是否返回竖屏主菜单？</param>
    public void API_BackToMenu(bool isMenuP = false)
    {
        if(isMenuP) 
        {
            /* 如果当前是横屏，就给竖过来，因为MenuP是竖屏的 */
            if(Screen.orientation == ScreenOrientation.Landscape) API_RotateScreen();
            API_LoadScene_SetName("SE_Menu_P");
        }
        else 
        {
            /* 如果当前是竖屏，就给竖过来，因为SE_Selector是横屏的 */
            if(Screen.orientation == ScreenOrientation.Portrait) API_RotateScreen();
            API_LoadScene_SetName("SE_Selector");
        }

        //走起
        API_LoadScene();
    }

    /// <summary>
    /// 返回关卡，就像浏览器的“后退”那样。
    /// </summary>
    /// <param name="haveAnimation">是否有过场动画</param>
    public void API_SceneGoBack(bool haveAnimation = false)
    {
        if(SceneHistory.Count < 2)
        {
            Debug.LogError("无法返回关卡：历史记录太少。");
            return;
        }
        
        if(haveAnimation)
        { 
            API_LoadScene_SetName(SceneHistory[SceneHistory.Count - 2]);
            API_LoadScene();
        }
        else
        {
            SceneHistory.Add(SceneHistory[SceneHistory.Count - 2]);
            SceneManager.LoadScene(SceneHistory[SceneHistory.Count - 2]);
        }
    }

    /// <summary>
    /// 播放按钮声音。
    /// </summary>
    /// <param name="index">ButtonSoundList数组的下标</param>
    public void API_PlayButtonSound(int index)
    {
        asAudioPlayer.PlayOneShot(ButtonSoundList[index]);
    }

    /// <summary>
    /// 开始转场
    /// 预期效果是，开始后若不停止，则转场会一直盖在最前面挡住相机
    /// </summary>
    /// <returns>实例化后的转场Prefab</returns>
    public GameObject API_Transition_Start()
    {
        if(goTransitionPrefab != null) Destroy(goTransitionPrefab);//如果当前正在有转场动画，就销毁
        goTransitionPrefab = Instantiate(TransitionPrefab);//实例化转场动画Prefab
        goTransitionPrefab.GetComponent<SRP_Transition>().StartTrans();//播放开始转场
        return goTransitionPrefab;
    }

    /// <summary>
    /// 停止转场
    /// </summary>
    public void API_Transition_Stop()
    {   //如果有可用的转场Prefab，就有序播放结束转场
        if(goTransitionPrefab != null) goTransitionPrefab.GetComponent<SRP_Transition>().EndTrans();
        else//如果没有可用的转场Prefab，就直接实例化并播放结束转场
        {
            goTransitionPrefab = Instantiate(TransitionPrefab);
            goTransitionPrefab.GetComponent<SRP_Transition>().EndTrans();
        }
    }

    /// <summary>
    /// 设置转场Prefab
    /// </summary>
    /// <param name="TransitionPrefab"></param>
    public void API_Transition_Set(GameObject TransitionPrefab)
    {
        this.TransitionPrefab = TransitionPrefab;
    }

    /// <summary>
    /// 使游戏胜利。
    /// </summary>
    /// <param name="NextSceneID">下一关卡的ID，非关卡名。</param>
    public void API_WinTheGame(string NextSceneID = "")
    {
        if(NextSceneID != "") UserManager.SetUserSubscribeClassBool(NextSceneID, true);
        if(goTransitionPrefab != null) Destroy(goTransitionPrefab);
        if(goWinPrefab != null) Destroy(goWinPrefab);
        goWinPrefab = Instantiate(WinPrefab);
    }

    /// <summary>
    /// 设置胜利Prefab
    /// </summary>
    /// <param name="WinPrefab"></param>
    public void API_WinTheGame_Set(GameObject WinPrefab)
    {
        this.WinPrefab = WinPrefab;
    }

    /// <summary>
    /// 失败
    /// </summary>
    public void API_FailTheGame()
    {
        if(goTransitionPrefab != null) Destroy(goTransitionPrefab);
        if(goFailPrefab != null) Destroy(goFailPrefab);
        goFailPrefab = Instantiate(FailPrefab);
    }

    /// <summary>
    /// 设置失败Prefab
    /// </summary>
    /// <param name="FailPrefab"></param>
    public void API_FailTheGame_Set(GameObject FailPrefab)
    {
        this.FailPrefab = FailPrefab;
    }

    /// <summary>
    /// 获取本地存档的密码，这与账户的本地数据是两码事。
    /// </summary>
    /// <returns>若没有设置过密码，则返回“NONE”。</returns>
    public string API_Local_GetPassword()
    {
        return API_Local_GetCustomString("password");
    }   

    /// <summary>
    /// 设置本地密码，这与账户的本地数据是两码事。
    /// </summary>
    /// <param name="NewPassword"></param>
    /// <returns>设置的密码。</returns>
    public string API_Local_SetPassword(string NewPassword)
    {
        return API_Local_SetCustomString("password", NewPassword);
    }

    /// <summary>
    /// 获取本地存档里的手机号，这与账户的本地数据是两码事。
    /// </summary>
    /// <returns>若没有设置过手机号，则返回“NONE”。</returns>
    public string API_Local_GetPhoneNumber()
    {
        return API_Local_GetCustomString("phone");
    }

    /// <summary>
    /// 获取本地存档里的自定义数据字符串。
    /// </summary>
    /// <param name="Key"></param>
    /// <returns>若没有设置过，则返回“NONE”。</returns>
    public string API_Local_GetCustomString(string Key)
    {
        return PlayerPrefs.GetString(Key, "NONE");
    }
    

    /// <summary>
    /// 设置本地存档里的自定义数据字符串。
    /// </summary>
    /// <param name="Key"></param>
    public string API_Local_SetCustomString(string Key, string Value)
    {
        PlayerPrefs.SetString(Key, Value);
        return Value;
    }

    /// <summary>
    /// 获取本地存档里的QQ号。
    /// </summary>
    /// <returns>若没有设置过，则返回“NONE”。</returns>
    public string API_Local_GetQQNumber()
    {
        return API_Local_GetCustomString("qq");
    }

    /// <summary>
    /// 设置本地手机号
    /// </summary>
    /// <param name="NewPhone"></param>
    /// <returns></returns>
    public string API_Local_SetPhoneNumber(string NewPhone)
    {
        return API_Local_SetCustomString("phone", NewPhone);
    }

    /// <summary>
    /// 获取记录在本地的用户名
    /// 如果没有记录过，就返回 NONE
    /// </summary>
    /// <returns></returns>
    public string API_Local_GetUserName()
    {
        return API_Local_GetCustomString("username");
    }

    /// <summary>
    /// 设置本地用户名
    /// </summary>
    /// <param name="NewName"></param>
    /// <returns></returns>
    public string API_Local_SetUserName(string NewName)
    {
        return API_Local_SetCustomString("username", NewName);
    }

    /// <summary>
    /// 设置本地QQ号
    /// </summary>
    /// <param name="NewQQ"></param>
    /// <returns></returns>
    public string API_Local_SetQQNumber(string NewQQ)
    {
        return API_Local_SetCustomString("qq", NewQQ);
    }

    /// <summary>
    /// 庆祝
    /// 返回实例化后的庆祝Prefab
    /// </summary>
    /// <returns></returns>
    public GameObject API_Toy_Celebrate()
    {
        return Instantiate(CelebratePrefab);
    }

    /// <summary>
    /// 设置庆祝Prefab
    /// </summary>
    /// <param name="NewCelebratePrefab"></param>
    public void API_Toy_Celebrate_Set(GameObject NewCelebratePrefab)
    {
        CelebratePrefab = NewCelebratePrefab;
    }

    /// <summary>
    /// 切换横竖屏模式。
    /// 若当前为横屏，调用后会变成竖屏。
    /// 在Editor模式下不会有效果。
    /// </summary>
    public void API_RotateScreen()
    {
        //判断平台
        if(!Application.isMobilePlatform)
        {   
            //Windows平台
            //交换横竖分辨率
            Screen.SetResolution(Screen.height, Screen.width, false);
        }
        else
        {
            //安卓或IOS平台
            //设置横屏竖屏
            if(Screen.orientation == ScreenOrientation.Portrait) Screen.orientation = ScreenOrientation.LandscapeLeft;
            else Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    /// <summary>
    /// 切换横竖屏模式。
    /// 在Editor模式下不会有效果。
    /// </summary>
    /// <param name="isGoHor">切换到横屏？</param>
    public void API_RotateScreen(bool isGoHor)
    {
        if(isGoHor == true)
        {
            if (!Application.isMobilePlatform)
            {
                if(Screen.height > Screen.width)
                {
                    API_RotateScreen();
                }
            }
            else
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
        }
        else
        {
            if (!Application.isMobilePlatform)
            {
                if(Screen.width > Screen.height)
                {
                    API_RotateScreen();
                }
            }
            else
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }
        }
    }

    /// <summary>
    /// 返回游戏是否正在载入
    /// </summary>
    /// <returns></returns>
    public bool API_IsGameLoading()
    {
        return bIsGameLoading;
    }

    /// <summary>
    /// 返回游戏是否正在载入
    /// 参数为bool，可以设置该变量。
    /// </summary>
    /// <param name="NewLoadStatus"></param>
    /// <returns></returns>
    public bool API_IsGameLoading(bool NewLoadStatus)
    {
        bIsGameLoading = NewLoadStatus;
        return API_IsGameLoading();
    }

    /// <summary>
    /// 获取触控移动器
    /// </summary>
    /// <returns></returns>
    public SRP_TouchMover API_GetTouchMover()
    {
        if(GameObject.FindGameObjectsWithTag("TouchMover").Length == 0)
        {
            Debug.LogAssertion("当前Scene中没有可用的TouchMover！");
            return null;
        }
        return GameObject.FindGameObjectsWithTag("TouchMover")[0].GetComponent<SRP_TouchMover>();
    }

    /// <summary>
    /// 读取Txt文件内容。若文件不存在，则返回"NULL"。
    /// </summary>
    /// <param name="FileFullPath"></param>
    /// <returns>txt文件内容</returns>
    [Obsolete("建议使用WTool.ReadFile()代替此函数", false)]
    public string API_ReadTxt(string FileFullPath)
    {
        if(File.Exists(FileFullPath) == false)
        {
            Debug.LogAssertion("文件不存在");
            return "NULL";
        }

        FileStream fs = new FileStream(FileFullPath, FileMode.Open, FileAccess.Read, FileShare.None);
        StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
        string str = sr.ReadToEnd();
        sr.Close();
        return str;
    }

    /// <summary>
    /// 删除文件。
    /// </summary>
    /// <param name="FileFullPath">文件全路径</param>
    /// <param name="inSilent">静音模式，文件不存在时就不会打印信息</param>
    /// <returns>是否删除成功</returns>
    public bool API_DeleteFile(string FileFullPath, bool inSilent = false)
    {
        if(File.Exists(FileFullPath) == false)
        {
            if(inSilent == false) print("文件不存在");
            return false;
        }

        File.Delete(FileFullPath);
        return true;
    }



    public delegate void DL_DownloadDelegate(bool isOK, string FileFullPath);//委托，作为回调函数的类型
    public bool DL_isDownloading { get {return isDownloading;} }//返回当前是否正在下载
    private bool isDownloading = false;
    
    /// <summary>
    /// 下载文件到本地，一次只能下载一个文件。
    /// 可使用DL_isDownloading查询是否正在下载。
    /// 可使用回调函数DL_DownloadDelegate(bool,string)委托，该函数参数为下载是否成功和下载文件全路径。
    /// 只有可以下载的文件才会返回正确的文件全路径，否则返回"-1"。
    /// </summary>
    /// <param name="URL"></param>
    /// <returns>返回所下载的文件的全路径</returns>
    public string DL_StartDownload(string URL)
    {
        return DL_Download(URL, false, null);
    }
    public string DL_StartDownload(string URL, bool isReplaceMode)
    {
        return DL_Download(URL, isReplaceMode, null);
    }
    public string DL_StartDownload(string URL, bool isReplaceMode, DL_DownloadDelegate Callback)
    {
        return DL_Download(URL, isReplaceMode, Callback);
    }
    public string DL_StartDownload(string URL, DL_DownloadDelegate Callback)
    {
        return DL_Download(URL, false, Callback);
    }

    private string DL_Download(string URL, bool isReplaceMode, DL_DownloadDelegate Callback)
    {
        string FileFullPath = Application.persistentDataPath + "/" + "DownloadedContent" + "/" + URL.Split('/')[URL.Split('/').Length - 1];

        if(isDownloading)
        {
            Debug.LogWarning("已有文件正在下载，无法下载其他文件。");
            Callback(false, FileFullPath);
            return "-1";
        }
        if(File.Exists(FileFullPath))
        {
            if(isReplaceMode == false)
            {
                print("文件已存在。" + "目标文件：" + FileFullPath);
                if(Callback != null) Callback(true, FileFullPath);//文件存在也归为下载成功
                return "-1";
            }
        }
        
        StartCoroutine(Download_IE(URL, isReplaceMode, Callback));
        return FileFullPath;
    }
    IEnumerator Download_IE(string URL,bool isReplaceMode, DL_DownloadDelegate Callback)
    {
        string savePath = Application.persistentDataPath + "/" + "DownloadedContent";
        
        string FileFullPath = savePath + "/" + URL.Split('/')[URL.Split('/').Length - 1];
        string FileName = URL.Split('/')[URL.Split('/').Length - 1];
        isDownloading = true;

        float TimeOut = 30f;
        float timer = 0f;

        print("文件开始下载。" + FileFullPath);
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(URL);
        unityWebRequest.timeout = 0;
        unityWebRequest.SendWebRequest();

        while(true)
        {
            timer += Time.deltaTime;
            if(unityWebRequest.isDone) break;
            if(timer >= TimeOut) break;
            yield return null;
        }

        if(unityWebRequest.isDone == false)
        {
            Debug.LogWarning("下载超时。" + TimeOut);
            isDownloading = false;
            if(Callback != null) Callback(false, FileFullPath);
            yield break;
        }

        if(unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("下载失败。" + URL);
            isDownloading = false;
            if(Callback != null) Callback(false, FileFullPath);
            yield break;
        }

        if(unityWebRequest.downloadHandler.data.Length <= 1)
        {
            Debug.LogWarning("文件不存在。" + "目标地址：" + unityWebRequest.url);
            isDownloading = false;
            if(Callback != null) Callback(false, FileFullPath);
            yield break;
        }

        byte[] results = unityWebRequest.downloadHandler.data;   //获取文件字节

        if (!Directory.Exists(savePath))//如果目录不存在
        {
            Directory.CreateDirectory(savePath);//创建目录
        }

        if(isReplaceMode == true) 
        {
            print("正在替换文件，目标文件：" + FileFullPath);
            File.Delete(FileFullPath);
        }

        FileInfo fileInfo = new FileInfo(FileFullPath);    //新建FileInfo请求
        FileStream fs = fileInfo.Create();  //在指定目录创建文件
        fs.Write(results, 0, results.Length);   //将字节数组内容复制到当前FileStream，fs.Write(字节数组, 开始位置, 数据长度);
        fs.Flush(); //清除此流的缓冲区，使得所有缓冲数据都写入到文件中。
        fs.Close(); //关闭文件流对象
        fs.Dispose(); //销毁文件对象

        isDownloading = false;
        if(Callback != null) Callback(true, FileFullPath);
        yield break;
    }

    /// <summary>
    /// 从本地读取AB包。
    /// 可搭配DL_StartDownload函数下载AB包，使用该函数读取。
    /// 参数为AB包名，无需输入后缀。
    /// 第二参数为回调函数，每载入完一个GameObject都会调用一次，回调函数参数为载入好的GameObject。
    /// </summary>
    /// <param name="ABName"></param>
    public void DL_LoadAssetBundleFromDisk(string ABName) => StartCoroutine(LoadAssetBundle_IE(ABName, null));
    public void DL_LoadAssetBundleFromDisk(string ABName, DL_LoadedAsset CallbackPerFile) => StartCoroutine(LoadAssetBundle_IE(ABName, CallbackPerFile));

    public delegate void DL_LoadedAsset(GameObject GO);

    IEnumerator LoadAssetBundle_IE(string ABName, DL_LoadedAsset Callback)
    {
        string savePath = Application.persistentDataPath + "/" + "DownloadedContent";
        string FileFullPath = savePath + "/" + ABName + ".ab";

        if(File.Exists(FileFullPath) == false)
        {
            print("AB包不存在，目标文件：" + FileFullPath);
            yield break;
        }

        var ASYNC = AssetBundle.LoadFromFileAsync(FileFullPath);
        
        while(true)
        {
            if(ASYNC.isDone) break;
            yield return null;
        }

        var ASYNC2 = ASYNC.assetBundle.LoadAllAssetsAsync<GameObject>();

        while(true)
        {
            if(ASYNC2.isDone) break;
            yield return null;
        }

        foreach(GameObject GO in ASYNC2.allAssets)
        {
            Instantiate(GO);
            if(Callback != null) Callback(GO);
        }

        ASYNC.assetBundle.Unload(false);

        yield break;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) if(API_IsGameLoading() == false) API_SceneGoBack(true);
    }
}
