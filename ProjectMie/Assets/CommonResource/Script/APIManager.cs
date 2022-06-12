using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class APIManager : MonoBehaviour
{
    [HideInInspector]
    public static APIManager API;   //静态类型变量，便于访问APIManager

    [Tooltip("按钮声音列表。")]
    public List<AudioClip> ButtonSoundList = new List<AudioClip>();

    [Tooltip("包含转场动画的Prefab。")]
    public GameObject TransitionPrefab;//这个是用来在Unity中拖拽赋值的
    private GameObject goTransitionPrefab;//将上面变量实例化后的引用

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
    private int iClassCollectionIndex = -1;//记录当前玩的是哪个大类，-1为没玩；大类如“学拼音”、“学成语”
    private int iSingleLessonIndex = -1;//记录当前玩的是大类中的哪个课程，如“拼音a”、“拼音b”
    private int iSublevelIndex = -1;//记录当前玩的是课程中的哪个小阶段，如“学”、“听”、“写”
    private bool bIsGameLoading = false;//记录当前是否正处于载入状态，由SRP_LoadingScene维护
    
    void Awake()
    {
        if(API != null) Destroy(gameObject);    //去重，防止多个APIManager出现
        API = this;     //为static变量赋值
        DontDestroyOnLoad(this);
        asAudioPlayer = gameObject.AddComponent<AudioSource>(); //初始化asAudioPlayer，这里使用AddComponent添加组件，省的在Unity那边添加了
    }

    void Start()
    {
        //NetManager.NET.UserData_Download();
        //API_LessonIndexCollection_Set(0,0,2);
        //NetManager.NET.UserData_AddElement(1, 3, 0);
        //API_UnlockNextSublevel();
        // API_LoadScene_SetName("SE_Test");
        // API_LoadScene();
    }

    /// <summary>
    /// 载入关卡
    /// </summary>
    public void API_LoadScene() 
    {   
        if(sLoadingSceneName == "NONE")
        {
            print("未设置载入地图名，已终止。");
            return;
        }

        //开始协程
        StartCoroutine(API_LoadScene_IE());
    }

    IEnumerator API_LoadScene_IE()
    {
        API_Transition_Start();//开始转场
        yield return new WaitForSeconds(0.5f);//等待转场动画完成

        //切换到SE_Loading这个Scene，第二个参数是当Scene加载完成后，关闭其他所有的Scene，相关的还有Additive模式，后面会用到
        SceneManager.LoadScene("SE_Loading");

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
    /// 返回主菜单
    /// 参数为true时返回竖屏主菜单，false时返回横屏主菜单。
    /// </summary>
    /// <param name="isMenuP"></param>
    public void API_BackToMenu(bool isMenuP = false)
    {
        //根据参数选择要返回的菜单
        if(isMenuP) 
        {
            if(Screen.orientation == ScreenOrientation.Landscape) API_RotateScreen();//如果当前是横屏，就给竖过来，因为MenuP是竖屏的
            API_LoadScene_SetName("SE_Menu_P");
        }
        else 
        {
            if(Screen.orientation == ScreenOrientation.Portrait) API_RotateScreen();//如果当前是竖屏，就给竖过来，因为MenuL是横屏的
            API_LoadScene_SetName("SE_Menu_L");
        }
        
        API_LessonIndexCollection_Set(Selector<int>(-1, -2, isMenuP), -1, -1);//初始化索引


        //走起
        API_LoadScene();
    }

    /// <summary>
    /// 播放按钮声音
    /// 参数是ButtonSoundList数组的下标。
    /// </summary>
    /// <param name="index"></param>
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
    /// 胜利
    /// </summary>
    public void API_WinTheGame()
    {
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
    /// 获取本地密码
    /// 若没有设置过，则返回“NONE”。
    /// </summary>
    /// <returns></returns>
    public string API_Local_GetPassword()
    {
        return PlayerPrefs.GetString("password","NONE");
    }   

    /// <summary>
    /// 设置本地密码
    /// 返回值是输入的密码
    /// </summary>
    /// <param name="NewPassword"></param>
    /// <returns></returns>
    public string API_Local_SetPassword(string NewPassword)
    {
        PlayerPrefs.SetString("password",NewPassword);
        return NewPassword;
    }

    /// <summary>
    /// 获取记录在本地的手机号
    /// 如果没有设置过，就返回 NONE
    /// </summary>
    /// <returns></returns>
    public string API_Local_GetPhoneNumber()
    {
        return PlayerPrefs.GetString("phone","NONE");
    }

    /// <summary>
    /// 获取记录在本地的自定义数据字符串
    /// 如果没有设置过，就返回 NONE
    /// </summary>
    /// <param name="Key"></param>
    /// <returns></returns>
    public string API_Local_GetCustomString(string Key)
    {
        return PlayerPrefs.GetString(Key, "NONE");
    }
    

    /// <summary>
    /// 设置记录在本地的自定义数据字符串
    /// </summary>
    /// <param name="Key"></param>
    /// <returns></returns>
    public void API_Local_SetCustomString(string Key, string Value)
    {
        PlayerPrefs.SetString(Key, Value);
    }

    /// <summary>
    /// 获取记录在本地的QQ号
    /// 如果没有设置过，就返回 NONE
    /// </summary>
    /// <returns></returns>
    public string API_Local_GetQQNumber()
    {
        return PlayerPrefs.GetString("qq","NONE");
    }

    /// <summary>
    /// 设置本地手机号
    /// </summary>
    /// <param name="NewPhone"></param>
    /// <returns></returns>
    public string API_Local_SetPhoneNumber(string NewPhone)
    {
        PlayerPrefs.SetString("phone",NewPhone);
        return NewPhone;
    }

    /// <summary>
    /// 获取记录在本地的用户名
    /// 如果没有记录过，就返回 NONE
    /// </summary>
    /// <returns></returns>
    public string API_Local_GetUserName()
    {
        return PlayerPrefs.GetString("username","NONE");
    }

    /// <summary>
    /// 设置本地用户名
    /// </summary>
    /// <param name="NewName"></param>
    /// <returns></returns>
    public string API_Local_SetUserName(string NewName)
    {
        PlayerPrefs.SetString("username",NewName);
        return NewName;
    }

    /// <summary>
    /// 设置本地QQ号
    /// </summary>
    /// <param name="NewQQ"></param>
    /// <returns></returns>
    public string API_Local_SetQQNumber(string NewQQ)
    {
        PlayerPrefs.SetString("qq",NewQQ);
        return NewQQ;
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
    /// 获取当前课程下标集合
    /// 返回向量三个参数分别表示当前位于哪个“大类”“课程”“课程中的小测试”
    /// </summary>
    /// <returns></returns>
    public Vector3Int API_LessonIndexCollection_Get()
    {
        return new Vector3Int(iClassCollectionIndex, iSingleLessonIndex, iSublevelIndex);
    }

    /// <summary>
    /// 设置当前课程索引，若参数为-2则表示对应项不变
    /// 可通过该函数获取当前玩家所在关卡位置，比如通过该函数实现解锁下一关。
    /// </summary>
    /// <param name="ClassCollectionIndex"></param>
    /// <param name="SingleLessonIndex"></param>
    /// <param name="SubLevelIndex"></param>
    /// <returns></returns>
    public Vector3Int API_LessonIndexCollection_Set(int ClassCollectionIndex = -2, int SingleLessonIndex = -2, int SubLevelIndex = -2)
    {
        if(ClassCollectionIndex != -2) iClassCollectionIndex = ClassCollectionIndex;
        if(SingleLessonIndex != -2) iSingleLessonIndex = SingleLessonIndex;
        if(SubLevelIndex != -2) iSublevelIndex = SubLevelIndex;
        return API_LessonIndexCollection_Get();
    }

    /// <summary>
    /// 解锁下一个Sublevel或SingleLesson。
    /// 如果当前Sublevel是该SingleLesson的最后一个Sublevel，那么程序就会去解锁下一个SingleLesson的第一个Sublevel。
    /// </summary>
    public void API_UnlockNextSublevel()
    {
        //!这里有个小问题，就是它是根据数据库数据去判断是否存在下一关卡的，如果数据库数据大于本地数据，就可能出现问题
        //所以关卡是处于“只可增不可减”的状态
        if(NetManager.NET == null)
        {
            print("未检测到NetManager的存在，终止操作。");
            return;
        }

        if(!NetManager.NET.IsLogin())
        {
            print("未登录账号，终止操作。");
            return;
        }
        
        //如果现在的Sublevel是这个SingleLesson中的最后一个Sublevel
        if(iSublevelIndex == NetManager.NET.GetUserData().ClassData[iClassCollectionIndex].LessonList[iSingleLessonIndex].isSublevelUnlock.Count - 1)
        {
            //print("本关卡是最后一个Sublevel！");
            //就把isTotalUnlock设为true，即：该SingleLesson中的所有Sublevel都通关了
            NetManager.NET.GetUserData().ClassData[iClassCollectionIndex].LessonList[iSingleLessonIndex].isTotalUnlock = true;

            //如果当前SingleLesson下标不等于最后一个SingleLesson下标，说明还有下一个SingleLesson，就去解锁下一个SingleLesson中的第一个Sublevel。判断一个SingleLesson是否解锁，是去判断该SingleLesson的第一个Sublevel是否解锁，所以这里只需要解锁下一个SingleLesson中的第一个Sublevel就可以了
            if(iSingleLessonIndex != NetManager.NET.GetUserData().ClassData[iClassCollectionIndex].LessonList.Count - 1)//看看现在的SingleLesson是不是最后一个SingleLesson
                NetManager.NET.GetUserData().ClassData[iClassCollectionIndex].LessonList[iSingleLessonIndex + 1].isSublevelUnlock[0] = true;//解锁下一SingleLesson的第一个Sublevel！
        }
        else
        {
            //print("本关卡不是最后一个Sublevel！");
            //如果现在不是最后一个Sublevel，就只解锁下一个Sublevel
            NetManager.NET.GetUserData().ClassData[iClassCollectionIndex].LessonList[iSingleLessonIndex].isSublevelUnlock[iSublevelIndex + 1] = true;
        }

        //更新课程数据
        NetManager.NET.UserData_Update();
    }

    /// <summary>
    /// 切换横竖屏模式
    /// 若当前为横屏，调用后会变成竖屏。
    /// </summary>
    public void API_RotateScreen()
    {
        //判断平台
        if(!Application.isMobilePlatform)
        {   
            //Windows平台
            //交换横竖分辨率
            Screen.SetResolution(Screen.resolutions[0].height, Screen.resolutions[0].width, false);
        }
        else
        {
            //安卓或IOS平台
            //设置横屏竖屏
            if(Screen.orientation == ScreenOrientation.Portrait) Screen.orientation = ScreenOrientation.LandscapeLeft;
            else Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    // /// <summary>
    // /// 解锁下一个SingleLesson
    // /// </summary>
    // public void API_UnlockNextSingleLesson()
    // {
    //     if(NetManager.NET == null)
    //     {
    //         print("未检测到NetManager的存在，终止操作。");
    //         return;
    //     }

    //     if(!NetManager.NET.IsLogin())
    //     {
    //         print("未登录账号，终止操作。");
    //         return;
    //     }
        
    //     if(iSingleLessonIndex == NetManager.NET.GetUserData().ClassData[iClassCollectionIndex].LessonList.Count - 1)
    //     {
    //         //如果现在已经是最后一个SingleLesson，就啥也不干
    //     }
    //     else
    //     {
    //         //如果现在不是最后一个SingleLesson，就解锁下一SingleLesson里的第一个Sublevel
    //         NetManager.NET.GetUserData().ClassData[iClassCollectionIndex].LessonList[iSingleLessonIndex + 1].isSublevelUnlock[0] = true;
    //     }

    //     //更新课程数据
    //     NetManager.NET.UserData_Update();
    // }

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
            print("当前Scene中没有可用的TouchMover！");
            return null;
        }
        return GameObject.FindGameObjectsWithTag("TouchMover")[0].GetComponent<SRP_TouchMover>();
    }

    /// <summary>
    /// 返回值选择器
    /// 作用是根据一个bool决定返回A还是B。
    /// 从UE4学来的，很方便吧！！
    /// 比如下载进度条，没下载完的时候要返回真实下载进度，下载完了就返回100%。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="isA"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Selector<T>(T A, T B, bool isA)
    {
        if(isA) return A;
        else return B;
    }
}
