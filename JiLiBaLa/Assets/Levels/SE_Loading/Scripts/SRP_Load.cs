using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SRP_Load : MonoBehaviour
{
    [Tooltip("包含Slider组件的GameObject，用于搞游戏载入进度条。")]
    public Slider Nslider;
    [Tooltip("包含Text组件的GameObject，用于显示下载进度，例：16MB/125MB")]
    public Text Ndltext;
    [Tooltip("包含Button组件的GameObject，用于显示取消下载")]
    public Button CancelBut;
    [HideInInspector]
    public float Nslidertargetvalue = 0;
    [Tooltip("虚假进度条载入速度因数，数值越高，虚假进度速度就越快，默认1。")]
    public float SliderSpeedFactor = 1.5f;
    
    void Awake()
    {
        Nslider.value = 0f;//初始化进度条默认值
    }

    private void Start()
    {
        //StartCoroutine(Policeman());//开始载入游戏协程
        Instantiate(GameManager.Instance.BlackOut);//播放黑屏切出效果
    }

    private void FixedUpdate()
    {
        Nslider.value = Mathf.MoveTowards(Nslider.value, Nslidertargetvalue, 0.01f * SliderSpeedFactor);
    }

    bool doOnce = false;//防止多点
    public void CONTROL_Cancel()
    {
        if(doOnce == false)//防止多点
        {
            StopAllCoroutines();
            GameManager.Instance.BackToMainmenu();
            doOnce = !doOnce;
        }
    }

    // IEnumerator Policeman()//载入游戏协程
    // {
    //     GameManager.Instance.isLoading = true;  //更新载入变量
    //     if (Application.CanStreamedLevelBeLoaded(GameManager.Instance.Nloadingscenename))   //如果本地存在对应关卡
    //     {   
    //         Destroy(CancelBut);//本地关卡不需要取消下载
    //         var map = SceneManager.LoadSceneAsync(GameManager.Instance.Nloadingscenename);  //异步载入本地关卡
    //         map.allowSceneActivation = false;   //设置：关卡载入好后，不许立刻切换到载入好的关卡。
    //                                             //!该设置会导致progress进度变量卡死在90%，直到为true才会继续载入。
    //         while (map.progress < 0.895)//当进度小于89.5%，即：未加载完地图时
    //         {
    //             Nslidertargetvalue = map.progress;//令虚假进度条等于progress
    //             yield return null;//等待一帧
    //         }
    //         //突破while，说明地图已经载入好了
    //         Nslidertargetvalue = 1f;//让虚假进度条等于1
    //         while (Nslider.value < 0.98)//当真正的进度条小于100%时(98%近似于100%)
    //         {
    //             //等待真进度条到100%
    //             yield return null;
    //         }
    //         //突破了while，说明虚假进度条已经100%了
    //         GameManager.Instance.Anim_BlackIn_Stay();//整个黑屏切出动画，并且切出后一直保持黑屏，所以你们需要在自己的关卡里手动添加黑窗切出效果
    //         yield return new WaitForSeconds(2f);//等待动画完成
    //         map.allowSceneActivation = true;//迎接下一关卡
    //     }
    //     else   //如果本地不存在对应关卡，即网络关卡
    //     {
    //         var map = SceneManager.LoadSceneAsync("SE_Network", LoadSceneMode.Additive);//以Addition的模式加载SE_Network关卡
    //         map.allowSceneActivation = false;
    //         UnityWebRequest WEB = UnityWebRequestAssetBundle.GetAssetBundle(SRP_DatabaseManager.Instance.sServerMainURL + "OnlineAssets/" + GameManager.Instance.Nloadingscenename + ".ab", SRP_DatabaseManager.Instance.iNetworkVersion, 0);//声明一个从php服务器下载对应关卡名的 AssetBundle 的请求
    //         WEB.timeout = 0;//设置timeout为0，因为无法控制timeout后的情况(比如timeout后重新连接)，所以避免使用timeout
    //         WEB.SendWebRequest();//发送请求
    //         if (WEB.result == UnityWebRequest.Result.ConnectionError) SceneManager.LoadScene("SE_LoginPage");//如果请求有错误就直接返回登录界面
    //         while (!WEB.isDone)//和载入本地关卡类似的while
    //         {
    //             Nslidertargetvalue = WEB.downloadProgress * 0.98f ;
    //             Ndltext.text = "正在下载：" + Nslider.value * 100 + "%" + "    " + WEB.downloadedBytes/1024/1024 + "MB / " +(WEB.downloadedBytes / WEB.downloadProgress) / 1024 / 1024 + "MB";
    //             yield return null;
    //         }
    //         AssetBundle BUNDLE = DownloadHandlerAssetBundle.GetContent(WEB);//从下载的内容里提取AssetBundle
    //         map.allowSceneActivation = true;//开s始切换关卡，由于载入关卡是Addition的形式，所以此时载入关卡没有被卸载，仍然存在
    //         Destroy(CancelBut);//下载完资源后把取消下载移除
    //         GameObject[] prefabs = BUNDLE.LoadAllAssets<GameObject>();//从AssetBundle中载入全部GameObject资源，包括Prefab和图片资源
    //         foreach (GameObject GO in prefabs)//在载入关卡实例化所有资源并将实例化后的Object移动到SE_Network关卡中
    //         {
    //             SceneManager.MoveGameObjectToScene(Instantiate(GO), SceneManager.GetSceneByName("SE_Network"));
    //         }
    //         yield return new WaitForSeconds(0.5f);//等0.5秒实例化时间，千千万万不要去掉！！！！！！！
    //         BUNDLE.Unload(false);//卸载AssetBundle(不是删除ab包，这个卸载貌似是指释放内存)，参数false代表不删除已实例化出的资源(若为true，实例化后的东西会都被Destroy)
    //         Nslidertargetvalue = 1f;//令虚假进度条等于1，和载入本地关卡类似
    //         while (Nslider.value < 0.99)
    //         {
    //             //等真进度条完成进度
    //             yield return null;
    //         }
    //         GameManager.Instance.Anim_BlackIn_Stay();//黑窗切出效果
    //         yield return new WaitForSeconds(2f);//等待黑窗动画完成
    //         //SceneManager.MoveGameObjectToScene(Instantiate(GameManager.Instance.BlackOut), SceneManager.GetSceneByName("SE_Network"));//在SE_Network关卡里创建黑窗切出效果
    //         SceneManager.UnloadSceneAsync("SE_loading");//卸载载入关卡
    //     }
    //     GameManager.Instance.isLoading = false;  //更新载入变量
    //     yield break;
    // }
}
