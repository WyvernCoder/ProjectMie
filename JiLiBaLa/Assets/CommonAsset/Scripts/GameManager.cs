using System.Collections;
using System.Collections.Generic;
//using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using UnityEngine.Networking;
/// <summary>
/// GameManager
///* 作用：为 全局游戏 提供按钮声音API、返回主菜单API、车厢信息、本地保存账号密码功能和切入切出黑屏动画API等。
/// </summary>

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameManager>() as GameManager;
            return _instance;
        }
    }
    
    [Header("对象引用部分")]

    [Tooltip("必须是 PRB_BlackIn Prefab。")]
    public GameObject BlackIn;

    [Tooltip("必须是 PRB_BlackInStayInBlack Prefab。")]
    public GameObject BlackInStayInBlack;

    [Tooltip("必须是 PRB_BlackOut Prefab。")]
    public GameObject BlackOut;
    
    [Tooltip("必须是 PRB_MissionFalied 的Prefab。(包括其子Prefab）")]
    public GameObject MissionFaliedPrefab;
    
    [Tooltip("必须是 PRB_MissionWin 的Prefab。(包括其子Prefab）")]
    public GameObject MissionWinPrefab;
    
    [Tooltip("载入时的动画Prefab。")]
    public GameObject LoadingPrefab;
    
    [Tooltip("必须是 PRB_Celebrate 的Prefab。(包括其子Prefab）")]
    public GameObject CelebratePrefab;
    
    [Tooltip("必须是 PRB_AdvPrint 。")]
    public GameObject AdvPrintPrefab;
    
    [HideInInspector]
    public GameObject CurrentPrefab;//当前的Prefab关卡，为null时表示没有进入任何Prefab关卡
    [HideInInspector]
    public GameObject CurrentPrefabAsset;//当前的Prefab关卡的资源文件，没有实例化的


    [Header("用户赋值部分")]

    [HideInInspector]
    public string sNum;//本地存储的账号
    [HideInInspector]
    public string sPassword;//本地存储的密码
    [HideInInspector]
    public string Nloadingscenename;//需要载入的关卡名；即使游戏已经开始，仍可通过该变量获取当前关卡名，用于重新开始关卡等。
    
    [Tooltip("按钮点击声音Prefab List，需要把能提供按钮点击声音的Prefab弄进去。")]
    public List<GameObject> ButtonClickSoundPrefabList = new List<GameObject>();

    [HideInInspector]
    public int iCurrentTrainBodyIndex;//当前关卡是第几节火车index，不算第一节
    [HideInInspector]
    public int iCurrentTrainWindowIndex;//当前关卡是第个窗户index，不算第一节
    [HideInInspector]
    public int iTotalTrainBodies;//总共有多少节车厢，不算第一节    
    [HideInInspector]
    public bool isLoading = false;//判断当前是否正在载入

    
    [HideInInspector]
    public GameObject IslandCollection;//岛屿合集Prefab

    private void Awake()
    {
        if (_instance != null) Destroy(gameObject);//去重，防止返回登录界面后刷出多余的GameManager
        GetInformation();//通过本地存档获取账号和密码信息
        
    }
    private void Start()
    {
        DontDestroyOnLoad(this);
        
    }


    /// <summary>
    /// 保存账号和密码至本地
    /// </summary>
    public void SaveInformation()
    {   //AC和PW对应的是账号和密码变量
        PlayerPrefs.SetString("AC", sNum);
        PlayerPrefs.SetString("PW", sPassword);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 从本地获取保存的账号和密码
    /// </summary>
    public void GetInformation()
    {   //AC和PW对应的是账号和密码变量，默认值啥也没有
        sNum = PlayerPrefs.GetString("AC", "");
        sPassword = PlayerPrefs.GetString("PW", "");
    }

    /// <summary>
    /// PlayButtonSound
    /// 作用：通过实例化对应Prefab实现播放按钮声音。
    /// 用法：直接调用，函数输入值为List中对应的第X个按钮声音Prefab，默认为0。
    /// </summary>
    /// <param name="index"></param>
    public void PlayButtonSound(int index = 0) 
    {
        if(index < ButtonClickSoundPrefabList.Count && index >= 0) Instantiate(ButtonClickSoundPrefabList[index]);
    }

    /// <summary>
    /// BackToMainmenu
    /// 功能：调用该函数后，会在黑屏切入切出的掩护下返回主菜单。
    /// 用法：直接调用。
    /// </summary>
    public void BackToMainmenu() => StartCoroutine(Backing()); 
    IEnumerator Backing()
    {
        //if(GameObject.FindObjectOfType<Camera>().GetComponent<AudioListener>() != null) Destroy(GameObject.FindObjectOfType<Camera>().GetComponent<AudioListener>());//AudioListener去重
        var A = DoLoading();
        CurrentPrefabAsset = null;
        Destroy(CurrentPrefab);//关闭当前Prefab关卡
        yield return new WaitForSeconds(1f);//1秒载入动画
        
        IslandCollection.SetActive(true);
        IslandCollection.GetComponent<SRP_IslandCollection>().TouchMoverPrefab.SetActive(true);
        IslandCollection.GetComponent<SRP_IslandCollection>().MainmenuControlCanvas.enabled = true;//显示左上角返回按钮
        Destroy(A);

        Anim_BlackOut();
        yield break;
        //TODO在关卡中可能会移动相机，返回主菜单后相机位置可能需要初始化
    }

    /// <summary>
    /// UnlockNextScene
    /// 作用：解锁下一关。
    /// 用法：直接调用。
    /// </summary>
    public void UnlockNextScene() => StartCoroutine(UnlockNextSceneIEN()); 
    IEnumerator UnlockNextSceneIEN()
    {

        yield break;
    }

    /// <summary>
    /// FailedTheMission
    /// 作用：调用后可直接进入任务失败界面。
    /// 用法：直接调用。
    /// </summary>
    public void FailedTheMission() => Instantiate(MissionFaliedPrefab);

    
    /// <summary>
    /// WinTheMission
    /// 作用：调用后可直接进入任务成功界面。
    /// 用法：直接调用。
    /// </summary>
    public void WinTheMission(bool bUnlockNextScene = true) 
    {
        if(bUnlockNextScene == true)UnlockNextScene();
        Instantiate(MissionWinPrefab);
    }

    public void Anim_BlackIn() => Instantiate(BlackIn);
    public void Anim_BlackIn_Stay() => Instantiate(BlackInStayInBlack);
    public void Anim_BlackOut() => Instantiate(BlackOut);
    public void DoCelebrate() => Instantiate(CelebratePrefab);
    public GameObject DoLoading()
    {
        return Instantiate(LoadingPrefab);
    }
    public void DoAdvPrint(string Text1 = "Hello,World!")
    {
        var print = Instantiate(AdvPrintPrefab);
        print.GetComponent<SRP_AdvPrint>().AdvPrint(Text1);
    }



































// //!以下是安卓状态栏部分

//     // Enums
//     public enum States
//     {
//         Unknown,
//         Visible,
//         VisibleOverContent,
//         TranslucentOverContent,
//         Hidden,
//     }

//     // !背景颜色
//     private const uint DEFAULT_BACKGROUND_COLOR = 0xff000000;



// #if UNITY_ANDROID
//     // Original Android flags
//     private const int VIEW_SYSTEM_UI_FLAG_VISIBLE = 0;                                        // Added in API 14 (Android 4.0.x): Status bar visible (the default)
//     private const int VIEW_SYSTEM_UI_FLAG_LOW_PROFILE = 1;                                // Added in API 14 (Android 4.0.x): Low profile for games, book readers, and video players; the status bar and/or navigation icons are dimmed out (if visible)
//     private const int VIEW_SYSTEM_UI_FLAG_HIDE_NAVIGATION = 2;                        // Added in API 14 (Android 4.0.x): Hides all navigation. Cleared when theres any user interaction.
//     private const int VIEW_SYSTEM_UI_FLAG_FULLSCREEN = 4;                                // Added in API 16 (Android 4.1.x): Hides status bar. Does nothing in Unity (already hidden if "status bar hidden" is checked)
//     private const int VIEW_SYSTEM_UI_FLAG_LAYOUT_STABLE = 256;                        // Added in API 16 (Android 4.1.x): ?
//     private const int VIEW_SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION = 512;        // Added in API 16 (Android 4.1.x): like HIDE_NAVIGATION, but for layouts? it causes the layout to be drawn like that, even if the whole view isn't (to avoid artifacts in animation)
//     private const int VIEW_SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN = 1024;                // Added in API 16 (Android 4.1.x): like FULLSCREEN, but for layouts? it causes the layout to be drawn like that, even if the whole view isn't (to avoid artifacts in animation)
//     private const int VIEW_SYSTEM_UI_FLAG_IMMERSIVE = 2048;                                // Added in API 19 (Android 4.4): like HIDE_NAVIGATION, but interactive (it's a modifier for HIDE_NAVIGATION, needs to be used with it)
//     private const int VIEW_SYSTEM_UI_FLAG_IMMERSIVE_STICKY = 4096;                // Added in API 19 (Android 4.4): tells that HIDE_NAVIGATION and FULSCREEN are interactive (also just a modifier)


//     //!这里会报错说声明但没使用过，但千万别注释掉，会打包不出来
//     private static int WINDOW_FLAG_FULLSCREEN = 0x00000400;
//     private static int WINDOW_FLAG_FORCE_NOT_FULLSCREEN = 0x00000800;
//     private static int WINDOW_FLAG_LAYOUT_IN_SCREEN = 0x00000100;
//     private static int WINDOW_FLAG_TRANSLUCENT_STATUS = 0x04000000;
//     private static int WINDOW_FLAG_TRANSLUCENT_NAVIGATION = 0x08000000;
//     private static int WINDOW_FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS = -2147483648; // 0x80000000; // Added in API 21 (Android 5.0): tells the Window is responsible for drawing the background for the system bars. If set, the system bars are drawn with a transparent background and the corresponding areas in this window are filled with the colors specified in getStatusBarColor() and getNavigationBarColor()

//     // Current values
//     private static int systemUiVisibilityValue;
//     private static int flagsValue;


// #endif

//     // Properties
//     private static States _statusBarState;
//     //        private static States _navigationBarState;

//     private static uint _statusBarColor = DEFAULT_BACKGROUND_COLOR;
//     //        private static uint _navigationBarColor = DEFAULT_BACKGROUND_COLOR;

//     private static bool _isStatusBarTranslucent; // Just so we know whether its translucent when hidden or not
//                                                  //        private static bool _isNavigationBarTranslucent;

//     private static bool _dimmed;
//     // ================================================================================================================
//     // INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

//     public static void AndroidStatusBar()
//     {
//         applyUIStates();
//         applyUIColors();
//     }

//     private static void applyUIStates()
//     {

// #if UNITY_ANDROID && !UNITY_EDITOR

//                 int newFlagsValue = 0;
//                 int newSystemUiVisibilityValue = 0;

//                 // Apply dim values
//                 if (_dimmed) newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LOW_PROFILE;

//                 // Apply color values
// //                if (_navigationBarColor != DEFAULT_BACKGROUND_COLOR || _statusBarColor != DEFAULT_BACKGROUND_COLOR) newFlagsValue |= WINDOW_FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS;
//                 if (_statusBarColor != DEFAULT_BACKGROUND_COLOR) newFlagsValue |= WINDOW_FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS;

//                 // Apply status bar values
//                 switch (_statusBarState) {
//                             case States.Visible:
//                             _isStatusBarTranslucent = false;
//                             newFlagsValue |= WINDOW_FLAG_FORCE_NOT_FULLSCREEN;
//                             break;
//                             case States.VisibleOverContent:
//                             _isStatusBarTranslucent = false;
//                             newFlagsValue |= WINDOW_FLAG_FORCE_NOT_FULLSCREEN | WINDOW_FLAG_LAYOUT_IN_SCREEN;
//                             newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN;
//                             break;
//                             case States.TranslucentOverContent:
//                             _isStatusBarTranslucent = true;//!透明
//                             newFlagsValue |= WINDOW_FLAG_FORCE_NOT_FULLSCREEN | WINDOW_FLAG_LAYOUT_IN_SCREEN | WINDOW_FLAG_TRANSLUCENT_STATUS;
//                             newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN;
//                             break;
//                             case States.Hidden:
//                             newFlagsValue |= WINDOW_FLAG_FULLSCREEN | WINDOW_FLAG_LAYOUT_IN_SCREEN;
//                             if (_isStatusBarTranslucent) newFlagsValue |= WINDOW_FLAG_TRANSLUCENT_STATUS;
//                             break;
//                 }

//                 // Applies navigation values
//                 /*
//                 switch (_navigationBarState) {
//                 case States.Visible:
//                 _isNavigationBarTranslucent = false;
//                 newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LAYOUT_STABLE;
//                 break;
//                 case States.VisibleOverContent:
//                 !Side effect: forces status bar over content if set to VISIBLE
//                 _isNavigationBarTranslucent = false;
//                 newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LAYOUT_STABLE | VIEW_SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION;
//                 break;
//                 case States.TranslucentOverContent:
//                 !Side effect: forces status bar over content if set to VISIBLE
//                 _isNavigationBarTranslucent = true;
//                 newFlagsValue |= WINDOW_FLAG_TRANSLUCENT_NAVIGATION;
//                 newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LAYOUT_STABLE | VIEW_SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION;
//                 break;
//                 case States.Hidden:
//                 newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_FULLSCREEN | VIEW_SYSTEM_UI_FLAG_HIDE_NAVIGATION | VIEW_SYSTEM_UI_FLAG_IMMERSIVE_STICKY;
//                 if (_isNavigationBarTranslucent) newFlagsValue |= WINDOW_FLAG_TRANSLUCENT_NAVIGATION;
//                 break;
//                 }
//                 */
//                 if (Screen.fullScreen) Screen.fullScreen = false;

//                 // Applies everything natively
//                 setFlags(newFlagsValue);
//                 setSystemUiVisibility(newSystemUiVisibilityValue);

// #endif
//     }

//     private static void applyUIColors()
//     {





// #if UNITY_ANDROID && !UNITY_EDITOR
//                 runOnAndroidUiThread(applyUIColorsAndroidInThread);

// #endif
//     }



// #if UNITY_ANDROID
//     private static void runOnAndroidUiThread(Action target)
//     {
//         using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//         {
//             using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
//             {
//                 activity.Call("runOnUiThread", new AndroidJavaRunnable(target));
//             }
//         }
//     }

//     private static void setSystemUiVisibility(int value)
//     {
//         if (systemUiVisibilityValue != value)
//         {
//             systemUiVisibilityValue = value;
//             runOnAndroidUiThread(setSystemUiVisibilityInThread);
//         }
//     }

//     private static void setSystemUiVisibilityInThread()
//     {
//         //Debug.Log("SYSTEM FLAGS: " + systemUiVisibilityValue);
//         using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//         {
//             using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
//             {
//                 using (var window = activity.Call<AndroidJavaObject>("getWindow"))
//                 {
//                     using (var view = window.Call<AndroidJavaObject>("getDecorView"))
//                     {
//                         view.Call("setSystemUiVisibility", systemUiVisibilityValue);
//                     }
//                 }
//             }
//         }
//     }

//     private static void setFlags(int value)
//     {
//         if (flagsValue != value)
//         {
//             flagsValue = value;
//             runOnAndroidUiThread(setFlagsInThread);
//         }
//     }

//     private static void setFlagsInThread()
//     {
//         //Debug.Log("FLAGS: " + flagsValue);
//         using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//         {
//             using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
//             {
//                 using (var window = activity.Call<AndroidJavaObject>("getWindow"))
//                 {
//                     window.Call("setFlags", flagsValue, -1); // (int)0x7FFFFFFF
//                 }
//             }
//         }
//     }

//     private static void applyUIColorsAndroidInThread()
//     {
//         using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//         {
//             using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
//             {
//                 using (var window = activity.Call<AndroidJavaObject>("getWindow"))
//                 {
//                     //Debug.Log("Colors SET: " + _statusBarColor);
//                     window.Call("setStatusBarColor", unchecked((int)_statusBarColor));
//                     //                                        window.Call("setNavigationBarColor", unchecked((int)_navigationBarColor));
//                 }
//             }
//         }
//     }



// #endif

//     // ================================================================================================================
//     // ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------
//     /*
//         public static States navigationBarState {
//                 get { return _navigationBarState; }
//                 set {
//                         if (_navigationBarState != value) {
//                                 _navigationBarState = value;
//                                 applyUIStates();
//                         }
//                 }
//         }
// */
//     public static States statusBarState
//     {
//         get { return _statusBarState; }
//         set
//         {
//             if (_statusBarState != value)
//             {
//                 _statusBarState = value;
//                 applyUIStates();
//             }
//         }
//     }

//     public static bool dimmed
//     {
//         get { return _dimmed; }
//         set
//         {
//             if (_dimmed != value)
//             {
//                 _dimmed = value;
//                 applyUIStates();
//             }
//         }
//     }

//     public static uint statusBarColor
//     {
//         get { return _statusBarColor; }
//         set
//         {
//             if (_statusBarColor != value)
//             {
//                 _statusBarColor = value;
//                 applyUIColors();
//                 applyUIStates();
//             }
//         }
//     }


//     /*
//         public static uint navigationBarColor {
//                 get { return _navigationBarColor; }
//                 set {
//                         if (_navigationBarColor != value) {
//                                 _navigationBarColor = value;
//                                 applyUIColors();
//                                 applyUIStates();
//                         }
//                 }
//         }
//         */













}
