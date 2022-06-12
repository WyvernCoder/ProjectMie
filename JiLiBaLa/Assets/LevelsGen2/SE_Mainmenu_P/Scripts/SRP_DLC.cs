using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SRP_DLC : MonoBehaviour
{
    [Header("对象赋值部分")]
    [HideInInspector]//焕新

    [Tooltip("安装进度条。")]
    public Slider DownloadSlider;

    [Tooltip("DLC按钮。")]
    public Button DLCButton;

    [Tooltip("图标组件")]
    public Image IconImage;

    [Tooltip("标题组件")]
    public GameObject TitleText;
    
    [Header("用户赋值部分")]
    [Tooltip("该窗口对应的关卡名。")]
    public string ABName = "SE_default";

    [Tooltip("安装介绍。")]
    public string InstallDescribe = "这将需要102.88MB的存储空间。";

    [Tooltip("卸载介绍。")]
    public string UninstallDescribe = "这将释放102.88MB的存储空间。";

    [Tooltip("已安装的图片。")]
    public Sprite InstalledImage;

    [Tooltip("正在卸载的图片。")]
    public Sprite UninstalledImage;

    [Tooltip("正在安装的图片。")]
    public Sprite InstallingImage;

    [Tooltip("未安装的图片。")]
    public Sprite NotInstallImage;

    private Image InstallStatueImage;
    
    [HideInInspector]
    public bool isDLCInstalled;
    
    [HideInInspector]
    public bool isUnistallMode = false;
    
    [Tooltip("是否是更新包？")]
    public bool isUpdateBundle = false;
    
    [HideInInspector]
    public SRP_PrivateCanvas PrivateCanvas;//用于刷新占用空间
    [HideInInspector]
    public SRP_Mainmenu_P_Manager MenuManager;//用于更新后刷新DLC菜单

    void Awake()
    {
        //寻找安装状态图标
        InstallStatueImage = gameObject.transform.Find("DownloadState/").gameObject.GetComponent<Image>();
        if(InstallStatueImage == null) print("未找到安装状态图标！");

        //寻找“我的”Canvas
        PrivateCanvas = GameObject.Find("Canvas_PrivateMenu").gameObject.GetComponent<SRP_PrivateCanvas>();
        if(PrivateCanvas == null) print("未找到“我的”Canvas！");

        //初始化安装图标
        InitalInstallStatue();

        //TODO嵌入SubTitle
        
        //初始化进度条
        DownloadSlider.value = 0;
        DownloadSlider.gameObject.SetActive(false);
    }

    public void CONTROL_OpenInstallMenu()
    {
        //在已安装的情况下什么都不弹出
        if(!isUpdateBundle) if(isDLCInstalled && !isUnistallMode) return;

        //打开DLC菜单
        //InstallPrefab.GetComponent<Canvas>().enabled = true;
        //var GO = Instantiate(InstallPrefab);

        //卸载模式的安装菜单
        if(isUnistallMode) 
        {   
            //显示菜单
            MenuManager.InstallWindowPrefab.GetComponent<Canvas>().sortingOrder = 10;

            //卸载模式的初始化菜单
            MenuManager.InstallWindowPrefab.GetComponent<SRP_DLC_InstallMenu>().SetUnistallMode(UninstallDescribe);
        }
        else
        {
            //显示菜单
            MenuManager.InstallWindowPrefab.GetComponent<Canvas>().sortingOrder = 10;

            //非卸载模式的初始化菜单
            MenuManager.InstallWindowPrefab.GetComponent<SRP_DLC_InstallMenu>().InitalContent(TitleText.GetComponent<TMP_Text>().text, IconImage.sprite, InstallDescribe);
            MenuManager.InstallWindowPrefab.GetComponent<SRP_DLC_InstallMenu>().SetInstallMode(InstallDescribe);
        }
        
        MenuManager.InstallWindowPrefab.GetComponent<SRP_DLC_InstallMenu>().DLC = this;
    }

    private void InitalInstallStatue()
    {
        //初始化安装图标
        isDLCInstalled = File.Exists(Application.persistentDataPath+"/DownloadContent/"+ABName+".ab");
        if(isDLCInstalled) InstallStatueImage.sprite = InstalledImage;
        else InstallStatueImage.sprite = NotInstallImage;
    }

    public void InitalThis(Sprite InstalledImageA, Sprite InstallingImageA, Sprite UninstallImageA, Sprite NotInstallImageA, string describetextA, bool isUpdateA)
    {
        isUpdateBundle = isUpdateA;
        if (InstalledImageA != null) InstalledImage = InstalledImageA;
        if (InstallingImageA != null) InstallingImage = InstallingImageA;
        if (UninstallImageA != null) UninstalledImage = UninstallImageA;
        if (NotInstallImageA != null) NotInstallImage = NotInstallImageA;
        gameObject.transform.Find("DescribeImage/TMP_DescribeText/").GetComponent<TMP_Text>().text = describetextA;
        if(isUpdateBundle)
        if(PlayerPrefs.GetInt("version") < SRP_DatabaseManager.Instance.iNetworkVersion) gameObject.transform.Find("DescribeImage/TMP_DescribeText/").GetComponent<TMP_Text>().text = "有新版本可用。";
        else gameObject.transform.Find("DescribeImage/TMP_DescribeText/").GetComponent<TMP_Text>().text = "暂无新版本可用。";
        InitalInstallStatue();
    }

    public void InitalThisNewGen(SRP_Mainmenu_P_Manager MenuManager, string PrefabName, string InstallDescribe, string UninstallDescribe, string TitleText)
    {
        this.MenuManager = MenuManager;
        ABName = PrefabName;
        this.InstallDescribe = InstallDescribe;
        this.UninstallDescribe = UninstallDescribe;
        this.TitleText.GetComponent<TMP_Text>().text = TitleText;
    }

    public void RecheckInstallStatue()
    {
        //重新检测图标状态
        isDLCInstalled = File.Exists(Application.persistentDataPath+"/DownloadContent/"+ABName+".ab");
        if(isDLCInstalled) InstallStatueImage.sprite = InstalledImage;
        else InstallStatueImage.sprite = NotInstallImage;
    }





    /// <summary>
    /// DLC安装部分
    /// </summary>
    /// <returns></returns>
    public void InstallDLC() => StartCoroutine(StartInstallDLC_IE());
    IEnumerator StartInstallDLC_IE()
    {
        //判断网络连接
        //只能判断目标服务器是否有效，不能判断php环境真的可用
        if(SRP_DatabaseManager.Instance.bServerConnection == false) 
        {
            print("无法连接至网络。");
            yield break;
        }

        //初始化进度条
        DownloadSlider.gameObject.SetActive(true);
        DownloadSlider.value = 0;
        DLCButton.interactable = false;

        //将安装图标设为安装中
        InstallStatueImage.sprite = InstallingImage;

        //资源下载后会被转移到这个文件夹里
        string SavePath = Application.persistentDataPath + "/DownloadContent";
        if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath); //如果目录不存在就创建

        

        //从php服务器下载ab包
        var WebRequest = UnityWebRequest.Get(SRP_DatabaseManager.Instance.sServerMainURL + "OnlineAssets" + "/" + ABName + ".ab");
        WebRequest.timeout = 0;
        //WebRequest.downloadHandler = new DownloadHandlerFile(Application.persistentDataPath + "/cache.ab");
         
        WebRequest.SendWebRequest();//发送请求
        while (DownloadSlider.value < 0.99f)
        {
            if(WebRequest.downloadProgress == -1)
            {
                print("下载失败，可能未发送下载请求。");
                yield break;
            }
            DownloadSlider.value = Mathf.Lerp(DownloadSlider.value,Mathf.MoveTowards(DownloadSlider.value, WebRequest.downloadProgress, 0.1f),0.1f);
            yield return null;
        }
        
        print(ABName + "已下载完成，正准备移动文件。");

        byte[] filebytes = WebRequest.downloadHandler.data;//获取下载文件的字节

        WebRequest.Dispose();//清理内存

        if (filebytes == null) 
        {
            print("下载文件" + ABName + "为空，无法继续移动，可能是无法获取文件字节。");
            yield break;
        }
        FileStream FS = new FileInfo(SavePath + "/" + ABName + ".ab").Create();//在对应目录创建新文件
        FS.Write(filebytes,0,filebytes.Length);//将文件字节写入新文件
        FS.Flush();//清除FS缓冲区并让所有缓冲数据写入到文件中
        FS.Close();//关闭FS文件流对象
        if (File.Exists(SavePath + "/" + ABName + ".ab"))//检查文件是否存在
        {   //判断文件大小
            if(GetFileSize(SavePath + "/" + ABName + ".ab") > 0f) print("文件已成功下载并移动完成。");
            else 
            {   
                print("文件似乎没有成功被下载，或者文件过小。" + GetFileSize(SavePath + "/" + ABName + ".ab"));
                UnistallDLC(true);//由于文件没有成功下载，所以需要将安装完成变成安装未完成
            }

        }
        else 
        {
            print("文件无法成功移动，未知原因。");
        }
        
        if (File.Exists(Application.persistentDataPath + "/cache.ab")) File.Delete(Application.persistentDataPath + "/cache.ab");

        //重新检测图标状态
        RecheckInstallStatue();

        //隐藏进度条
        DownloadSlider.gameObject.SetActive(false);
        DLCButton.interactable = true;
        yield break;
    }

    public void UnistallDLC(bool DontRemoveContent = false) => StartCoroutine(StartUnistallDLC_IE(DontRemoveContent));
    IEnumerator StartUnistallDLC_IE(bool DontRemoveContent = false)
    {
        //初始化进度条
        DownloadSlider.gameObject.SetActive(true);
        DownloadSlider.value = 0;

        //将安装图标设为安装中
        InstallStatueImage.sprite = UninstalledImage;

        //删除文件
        File.Delete(Application.persistentDataPath + "/DownloadContent/" + ABName + ".ab");
        isDLCInstalled = false;

        if(File.Exists(Application.persistentDataPath + "/DownloadContent/" + ABName + ".ab"))
        print("删除失败。");
        else print("删除完成。");

        while (DownloadSlider.value < 0.99f)
        {
            DownloadSlider.value = Mathf.Lerp(DownloadSlider.value,Mathf.MoveTowards(DownloadSlider.value, 1f, 0.1f),0.3f);
            yield return null;
        }
        
        if(DontRemoveContent == false)
        Destroy(gameObject);
        
        yield break;
    }


    public void InstallUpdate() => StartCoroutine(InstallUpdate_IE());
    IEnumerator InstallUpdate_IE()
    {
        //判断网络连接
        //只能判断目标服务器是否有效，不能判断php环境真的可用
        if(SRP_DatabaseManager.Instance.bServerConnection == false) 
        {
            print("无法连接至网络，update包停止下载。");
            yield break;
        }

        //初始化进度条
        DownloadSlider.gameObject.SetActive(true);
        DownloadSlider.value = 0;
        DLCButton.interactable = false;

        //将安装图标设为安装中
        InstallStatueImage.sprite = InstallingImage;

        //资源下载后会被转移到这个文件夹里
        string SavePath = Application.persistentDataPath + "/DownloadContent";
        if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath); //如果目录不存在就创建

        //从php服务器下载ab包
        var WebRequest = UnityWebRequest.Get(SRP_DatabaseManager.Instance.sServerMainURL + "OnlineAssets" + "/" + ABName + ".ab");
        WebRequest.timeout = 0;
        WebRequest.SendWebRequest();//发送请求
        while (DownloadSlider.value < 0.49f)//下载只占50%，另50%是解压
        {
            if(WebRequest.downloadProgress == -1)
            {
                print("下载失败，可能未发送下载请求。");
                yield break;
            }
            DownloadSlider.value = Mathf.Lerp(DownloadSlider.value,Mathf.MoveTowards(DownloadSlider.value, WebRequest.downloadProgress*0.5f, 0.1f),0.1f);
            yield return null;
        }
        
        if (File.Exists(Application.persistentDataPath + "/DownloadContent/" + ABName + ".ab")) File.Delete(Application.persistentDataPath + "/DownloadContent/" + ABName + ".ab"); //如果文件已存在就删除
        

        byte[] filebytes = WebRequest.downloadHandler.data;//获取下载文件的字节
        if (filebytes == null) 
        {
            print("下载文件" + ABName + "为空，无法继续移动，可能是无法获取文件字节。");
            yield break;
        }

        FileStream FS = new FileInfo(SavePath + "/" + ABName + ".ab").Create();//在对应目录创建新文件
        FS.Write(filebytes,0,filebytes.Length);//将文件字节写入新文件
        FS.Flush();//清除FS缓冲区并让所有缓冲数据写入到文件中
        FS.Close();//关闭FS文件流对象
        if (File.Exists(SavePath + "/" + ABName + ".ab"))//检查文件是否存在
        {   //!文件过小，无法判断文件大小，无论如何都会返回0
            // if(GetFileSize(SavePath + "/" + ABName + ".ab") > 0f) print("文件已成功下载并移动完成。");
            // else 
            // {
            //     print("文件似乎没有成功被下载。");
            //     UnistallDLC(true);//由于文件没有成功下载，所以需要将安装完成变成安装未完成
            //     yield break;
            // }

        }
        else 
        {
            print("文件无法成功移动，未知原因。");
        }

        if(!File.Exists(Application.persistentDataPath + "/DownloadContent/" + "update" + ".ab"))
        {
            print("寄了，没有找到update.ab，你可能误把普通DLC的更新选项勾了。");
            yield break;
        }
        AssetBundleCreateRequest AB = AssetBundle.LoadFromFileAsync(Application.persistentDataPath + "/DownloadContent/" + "update" + ".ab");
        while(!AB.isDone) yield return null;
        if(AB.assetBundle == null)
        {
            print("更新文件没有内容或断网情况！");
            yield break;
        }
        var AB2 = AB.assetBundle.LoadAllAssetsAsync();
        while(!AB2.isDone) yield return null;
        foreach(var GO in AB2.allAssets) Instantiate(GO);
        AB.assetBundle.Unload(false);
        yield return new WaitForSeconds(0.2f);

        while(DownloadSlider.value <= 0.99f) 
        {
            DownloadSlider.value = Mathf.Lerp(DownloadSlider.value,Mathf.MoveTowards(DownloadSlider.value, 1f, 0.1f),0.2f);
            yield return null;
        }

        print("更新成功。");
        //重新检测图标状态
        RecheckInstallStatue();

        //刷新DLC内容
        //MenuManager.RefreshDLCContent();

        //隐藏进度条
        DownloadSlider.gameObject.SetActive(false);
        DLCButton.interactable = true;
        yield break;
    }

    public int GetDirectionSize(string path)
    {
        int result = 0;
        var dirSource = new DirectoryInfo(path);    //获取目录信息
        foreach(FileInfo FI in dirSource.GetFiles())    //从目录信息中读取文件信息
        {
            result += (int)Mathf.Round(FI.Length/1000000f);
        }
        return result;
    }

    public float GetFileSize(string path)
    {   //返回的是MB
        return Mathf.Round(new FileInfo(path).Length/1000000f);
    }
}
