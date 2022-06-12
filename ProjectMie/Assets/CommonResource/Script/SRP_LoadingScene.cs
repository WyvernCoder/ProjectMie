using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.IO;//用于文件的读写复制等功能

public class SRP_LoadingScene : MonoBehaviour
{
    public string SavePath = "DownloadableContent";
    public Slider ProcessBar;
    public TMP_Text TMPText;
    private AssetBundle AB;//用于记住当前载入的AB包用的

    void Awake()
    {
        DontDestroyOnLoad(this);//因为它要负责多个关卡的切换，所以它不能被销毁，关卡切换完后手动清理掉就可以了
    }

    void Start()
    {
        StartCoroutine(GoLoading_IE());//!这个协程不能放在Awake里，会导致allowSceneActivation失灵的BUG
    }

    IEnumerator GoLoading_IE()
    {
        //看看咱们的API在不在线，不在线当然不行啦
        if (APIManager.API == null)
        {
            print("游戏载入必须依赖APIManager，终止。");
            SweepTrash();//清理自己
            yield break;
        }

        APIManager.API.API_IsGameLoading(true);//设置当前状态为载入状态

        print("——↓载入关卡↓——");//用于区分Console里关于载入的Debug信息，咱们单独把这啰嗦的载入给圈出来

        //判断要载入的关卡是否存在，存在就直接载入，不存在就给它弄存在了再回到前面的载入
        if (Application.CanStreamedLevelBeLoaded(APIManager.API.API_LoadScene_GetName()))
        {
            //*存在本地关卡

            if (AB == null) print("要载入的关卡是本地关卡，正在载入...");
            else print("要载入的关卡是从AB包解压出来的关卡，正在载入...");
            
            //本地关卡的载入不需要进度条
            Destroy(ProcessBar.gameObject.transform.parent.gameObject);

            //异步加载关卡
            AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(APIManager.API.API_LoadScene_GetName(), LoadSceneMode.Single);

            //当异步关卡载入完成后，禁止立刻切换至该关卡，使其载入进度停止在90%
            //之所以要这样做是因为加载关卡的速度实在是太快了，快到看起来不正常了
            sceneLoad.allowSceneActivation = false;

            //等待加载完成
            while (sceneLoad.progress <= 0.89f)//加载进度小于90%时
            {
                yield return null;//等待一帧
            }

            //加载完成后，做一个假的快速进度条，其实就是为了实现加载完等一会才会进入下一关
            float fakeProgress = 0f;//弄一个假的进度
            while (fakeProgress <= 0.99f)    //让fakeProgress的值以每帧靠近10%的速度去接近1
            {
                fakeProgress = Mathf.Lerp(fakeProgress, 1f, 0.04f);
                yield return null;//等待一帧
            }

            //走起！
            print("关卡" + APIManager.API.API_LoadScene_GetName() + "加载完成！");
            sceneLoad.allowSceneActivation = true;
        }
        else
        {
            //*不存在本地关卡

            print("未检测到关卡，准备下载。");

            //这个变量表示保存位置，其中persistentDataPath是Unity定义的位置，咱们不要去管它，只需要知道它属于咩咩学语的缓存位置就可以了
            //真想探究它在哪的话，print一下就好了，它在各个平台的位置是不一样的
            //Windows平台，在LocalLow里
            string savePath = Application.persistentDataPath + "/" + SavePath;

            //看看要移动到的目录是否存在，如果不存在就创建一个
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

            //判断本地是否存在对应AB包
            if (!File.Exists(savePath + "/" + APIManager.API.API_LoadScene_GetName() + ".ab"))
            {
                //*如果不存在，就下载

                //定义一个WebRequest下载请求
                var WebRequest = UnityWebRequest.Get(NetManager.NET.GetPHPAddress() + "OnlineAssets/" + APIManager.API.API_LoadScene_GetName() + ".ab");

                //由于咱们服务器速度都很慢很慢，就不需要timeout了...
                WebRequest.timeout = 0;

                //发送WebRequest下载请求
                WebRequest.SendWebRequest();

                //正在下载
                print("===开始下载===");
                while (ProcessBar.value < 0.99f)
                {
                    ProcessBar.value = Mathf.Clamp(Mathf.Lerp(ProcessBar.value, APIManager.API.Selector<float>(1f, WebRequest.downloadProgress, WebRequest.isDone), 0.01f), 0f, 1f);
                    TMPText.text = "DOWNLOADING..." + (int)(ProcessBar.value*100f) + "%";
                    yield return null;
                }

                print(APIManager.API.API_LoadScene_GetName() + ".ab" + " 已下载完成。");
                print("===完成下载===");

                //获取下载完成的文件的字节数据
                byte[] filebytes = WebRequest.downloadHandler.data;

                if (filebytes.Length <= 2048)//如果文件小于2MB
                {
                    print("没有下载任何文件，返回主菜单。");
                    SceneManager.LoadScene("SE_Menu_P");
                    APIManager.API.API_RotateScreen();//旋转屏幕
                    APIManager.API.API_IsGameLoading(false);//设置载入标签
                    APIManager.API.API_Transition_Stop();//停止转场
                    yield break;
                }

                //将字节数据转换成文件，也就是咱们要保存的文件；这部分在微软C#文档上有讲
                FileStream FS = new FileInfo(savePath + "/" + APIManager.API.API_LoadScene_GetName() + ".ab").Create();//创建一个文件流
                FS.Write(filebytes, 0, filebytes.Length);//将字节数据写入缓冲区
                FS.Flush();//该操作会在清除缓冲区的同时，将缓冲区数据写入新文件
                FS.Close();//关闭文件流

                //再看看有没有文件
                if (!File.Exists(savePath + "/" + APIManager.API.API_LoadScene_GetName() + ".ab"))
                {
                    print("文件创建失败！");
                    yield break;
                }
            }

            //*从AB包里加载Scene
            //把AB包内容加载到内存里
            AB = AssetBundle.LoadFromFile(savePath + "/" + APIManager.API.API_LoadScene_GetName() + ".ab");

            //加载AB包后，再看看有没有对应的关卡名
            if (!Application.CanStreamedLevelBeLoaded(APIManager.API.API_LoadScene_GetName()))
            {
                print("无法从AB包中加载" + APIManager.API.API_LoadScene_GetName() + "关卡，准备读取AB包中的第一个关卡。");

                if (AB.GetAllScenePaths().Length == 0)
                {
                    print("你这AB包不保熟啊，里面压根就没有关卡，准备返回主菜单。");
                    SweepTrash(true);//善后工作
                    APIManager.API.API_BackToMenu();
                    yield break;
                }

                //从关卡路径列表里找到第一个关卡路径
                string newSceneName = AB.GetAllScenePaths()[0];

                //去掉路径的.unity和前面的路径
                newSceneName = newSceneName.Substring(0, newSceneName.Length - 6);//去掉.unity
                newSceneName = newSceneName.Substring(newSceneName.LastIndexOf("/") + 1);//去掉最后一个"/"前面的所有字符

                //重定向关卡名，相当于上面的步骤是用于获取正确的关卡名的
                APIManager.API.API_LoadScene_SetName(newSceneName);

                //看看重定向后的关卡还能不能载入
                if (!Application.CanStreamedLevelBeLoaded(APIManager.API.API_LoadScene_GetName()))
                {
                    print("关卡仍然无法被加载，出现未知错误，准备返回主菜单。");
                    SweepTrash(true);//善后工作
                    APIManager.API.API_BackToMenu();
                    yield break;
                }

                //重定向完成后，再重来一遍。
                StartCoroutine(GoLoading_IE());
                yield break;
            }
            else
            {
                //既然有关卡了，咱们就重新开始载入流程，当作本地关卡载入。
                StartCoroutine(GoLoading_IE());
                yield break;
            }
        }

        //善后工作
        SweepTrash();
        yield break;
    }

    private void SweepTrash(bool DontDestory = false)
    {
        APIManager.API.API_Transition_Stop();//停止转场
        APIManager.API.API_IsGameLoading(false);//更新载入状态
        if(APIManager.API.API_LoadScene_GetName() == "SE_Menu_L" || APIManager.API.API_LoadScene_GetName() == "SE_Menu_P") APIManager.API.API_LoadScene_SetName("NONE");//如果返回的是主菜单，就初始化LoadScene
        if (AB != null) AB.Unload(false);//如果AB包有效，就给它Unload掉。参数false是指不销毁AB包解压到内存的东西.
        print("——↑载入关卡↑——");
        if (!DontDestory) Destroy(gameObject);//善后工作
    }

    public void CONTROL_BarUpdate()
    {
        //什么都不干，用于解决Slider不更新的BUG
        return;
    }
}
