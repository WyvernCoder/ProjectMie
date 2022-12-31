using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SRP_Loading : MonoBehaviour
{
    //public Slider ProcessBar;
    private string DownloadURL;
    private string savePath;
    private string filePath;
    string platformFullName = "";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        APIManager.GENERATE_BODY();
        DownloadURL = APIManager.API.HttpServer + "/OnlineAssets/";
    }

    void Start()
    {
        // 更新平台名称用以下载正确平台的 AB 包，如”ABC_Windows.ab“。
        UpdatePlatformName();

        // 设置保存目录
        // persistentDataPath 这个路径在 Win 和 Android 平台都是安全的，不会出现无权限的情况，如果好奇，可以 Debug 一下看看这个目录指向的是哪里。
        // 我们把下载的 AB 包放在这个安全目录下的 DownloadedContent 文件夹里方便归类。
        savePath = Path.Combine(Application.persistentDataPath, "DownloadedContent");

        // 设置文件路径
        // 文件路径指向要载入 ab 包的路径。
        filePath = Path.Combine(savePath, APIManager.API.API_LoadScene_GetName() + ".ab");

        // 看看目录是否存在，如果不存在就创建一个
        if (Directory.Exists(savePath) == false) Directory.CreateDirectory(savePath);

        // 走起
        StartCoroutine(LoadStream());
    }

    IEnumerator LoadStream()
    {
        // 试图下载文件
        yield return StartCoroutine(Download_IE());

        // 下载后验证一下文件是否存在
        if (File.Exists(filePath) == false)
        {
            Debug.LogError("已下载的文件不存在，可能下载过程出现问题，正在返回主菜单。");
            APIManager.API.API_LoadScene_SetName("SE_Selector");
            var load = SceneManager.LoadSceneAsync("SE_Selector");
            while (load.isDone == false) yield return null;
            APIManager.API.API_IsGameLoading(false);
            APIManager.API.API_Transition_Stop();
            Destroy(gameObject);
            yield break;
        }

        // 载入AB包
        yield return StartCoroutine(LoadAB_IE());

        // 载入关卡
        var scene = SceneManager.LoadSceneAsync(APIManager.API.API_LoadScene_GetName());
        while (scene.isDone == false) yield return null;
        APIManager.API.API_Transition_Stop();
        APIManager.API.API_IsGameLoading(false);

        Destroy(gameObject);
        yield break;
    }

    IEnumerator Download_IE()
    {
        // 判断文件是否存在，如果存在，就直接 return 掉
        if (File.Exists(filePath))
        {
            //Debug.Log("文件已存在，不需要下载。");
            yield break;
        }

        // 设置下载地址
        var downloadURL = Path.Combine(DownloadURL, APIManager.API.API_LoadScene_GetName() + platformFullName + ".ab");
        downloadURL = downloadURL.Replace('\\', '/');   // HTML 不太喜欢右斜杠

        // 从下载地址下载文件
        var downloader = UnityWebRequest.Get(downloadURL);  // 配置GET类型的 WebRequest
        downloader.SendWebRequest();    // 发送请求
        while (downloader.isDone == false)     // 当进度条小于 0.99 或者未下载完成时开始死循环
        {
            // 每次循环都更新一下进度条，用插值实现流畅进度条
            //ProcessBar.value = Mathf.Lerp(ProcessBar.value, downloader.downloadProgress, Time.deltaTime);
            yield return null;
        }

        // 获取下载的二进制数据
        byte[] filebytes = downloader.downloadHandler.data;

        // 如果下载的数据小于 1KB
        if (filebytes.Length <= 1024)
        {
            Debug.LogWarning("下载文件不存在，现在返回主菜单。");
            APIManager.API.API_LoadScene_SetName("SE_Selector");
            APIManager.API.API_LoadScene();
            Destroy(gameObject);
            yield break;
        }

        FileStream fs = new FileInfo(filePath).Create();    // 创建空文件并搞个对于它的数据流
        fs.Write(filebytes, 0, filebytes.Length);   // 将二进制数据写入缓冲区
        fs.Flush(); // 将缓冲区数据写入文件并清除缓冲区
        fs.Close(); // 关闭数据流

        yield break;
    }

    IEnumerator LoadAB_IE()
    {
        // 将 AB 包数据加载进内存
        AssetBundle AB = AssetBundle.LoadFromFile(filePath);

        if (AB == null)
        {
            var newPath = Path.ChangeExtension(filePath, "error");
            if (File.Exists(newPath)) File.Delete(newPath);
            File.Move(filePath, newPath);
            Debug.LogError("无法打开该AB包，可能是文件损坏，已标记该文件并返回主菜单，该文件将在下次载入时重新下载。");
            APIManager.API.API_LoadScene_SetName("SE_Selector");
            var load = SceneManager.LoadSceneAsync("SE_Selector");
            while (load.isDone == false) yield return null;
            APIManager.API.API_IsGameLoading(false);
            APIManager.API.API_Transition_Stop();
            Destroy(gameObject);
            yield break;
        }

        // 判断 AB 包中有无关卡
        if (AB.GetAllScenePaths().Length == 0)
        {
            AB.Unload(true);    // 清除内存中的 AB 包资源
            Debug.LogError("AB 包里没有任何关卡，返回主菜单。");
            APIManager.API.API_LoadScene_SetName("SE_Selector");
            var load = SceneManager.LoadSceneAsync("SE_Selector");
            while (load.isDone == false) yield return null;
            APIManager.API.API_IsGameLoading(false);
            APIManager.API.API_Transition_Stop();
            Destroy(gameObject);
        }

        // 看看 AB 包里有没有指定关卡，如果AB包中没有指定的关卡，就读取包中第一个关卡。
        if (Application.CanStreamedLevelBeLoaded(APIManager.API.API_LoadScene_GetName()) == false)
        {
            var sceneName = Path.GetFileNameWithoutExtension(AB.GetAllScenePaths()[0]);
            APIManager.API.API_LoadScene_SetName(sceneName);
        }

        yield break;
    }

    /// <summary>
    /// 更新平台名称
    /// AB 包是分平台的，如果使用我给的AB打包脚本，那么你打出来的AB包名会以平台名称作为结尾，比如”ABC_Windows.ab“。
    /// </summary>
    void UpdatePlatformName()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer: platformFullName = "Windows"; break;
            case RuntimePlatform.WindowsEditor: platformFullName = "Windows"; break;
            case RuntimePlatform.Android: platformFullName = "Android"; break;
            case RuntimePlatform.IPhonePlayer: platformFullName = "iOS"; break;
        }
        if (platformFullName != "") platformFullName = "_" + platformFullName;
    }
}
