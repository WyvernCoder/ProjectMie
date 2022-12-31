using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SRP_Menu_P_Manager : MonoBehaviour
{
    // !每添加一个切换页面函数，都要把那个函数填入CONTROL_BackPage()的数组里以注册返回页
    // !每添加一个过渡动画为水平动画的页面，都要把那个页面的index加入MenuP_PageIndex()的switch里，否则从对应页面返回时将会使用预换页的动画

    public GameObject CanvasRoot;//拖拽赋值，我们将会遍历这个GO下的所有子GO，这些子GO必须带有Canvas组件，这么做是为了方便制作切换页面
    public GameObject NAVBar;//拖拽赋值，一些页面不需要显示导航栏
    public GameObject NAVHead;//拖拽赋值，状态栏需要切换黑白主题
    public Animator AnimCtrl;//拖拽赋值，用于控制菜单栏指标动画
    private int iCurrentPageIndex = 0;//当前页(下一页)的索引
    private int iLastPageIndex = -1;//上一页的索引
    private List<GameObject> CanvasList = new List<GameObject>();//存放每个页的Canvas GameObject的List
    private List<RectTransform> CanvasTransformList = new List<RectTransform>();//存放每个页的Canvas GameObject的List
    public UnityEngine.Video.VideoPlayer VP;//视频播放器
    public TMPro.TMP_Text VP_Author;
    public TMPro.TMP_Text VP_Describe;
    public Animator NAVBar_Animator;
    public string VideoNullUrl;
    private bool isSwitchingPage = false;//只能一次翻一页
    private bool reverseSweepDirOnce = false;//水平过渡动画是否相反方向，仅在返回页中使用
    //private int iCachePageIndex = -1;

    void Awake()
    {
        APIManager.GENERATE_BODY();
    }

    void Start()
    {
        /* 初始化CanvasList，把所有页面都加进去，便于换页控制 */
        foreach (Transform TM in CanvasRoot.transform) 
        {
            CanvasList.Add(TM.gameObject);

            try
            {
                CanvasTransformList.Add(TM.gameObject.GetComponent<SRP_CanvasTransformGetter>().CanvasTransform.GetComponent<RectTransform>());
            }
            catch
            {
                Debug.LogAssertion(TM.gameObject.name + "未按照要求添加一层Transform，初始化终止。");
                Destroy(this);
            }

            TM.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        }

        if (CanvasList.Count == 0)
        {
            Debug.LogAssertion("未找到任何可用页！");
            return;
        }

        /* 初始化VP */
        GetComponent<UnityEngine.Video.VideoPlayer>().url = APIManager.API.HttpServer + "/OnlineVideo/null.mp4";
        VP = GetComponent<UnityEngine.Video.VideoPlayer>();

        MenuP_PageIndex(MenuP_PageIndex());//开屏首页

        StartCoroutine(ChangePageInFade());//启动渐变翻页协程
        StartCoroutine(ChangePageInSweep());//启动水平翻页协程
    }

    /// <summary>
    /// 获取当前页的索引
    /// </summary>
    /// <returns></returns>
    public int MenuP_PageIndex()
    {
        return iCurrentPageIndex;
    }

    /// <summary>
    /// 翻页至参数索引
    /// !规定：0为首页，1为学习页，2为我的，3为注册，4为登录，5为播放视频页，6为子公告页，7为配音秀，8为活动，9为打卡，10为视频课程选择页
    /// !11为视频选择页，12为视频详细页
    /// </summary>
    /// <param name="INDEX">页面下标</param>
    /// <param name="useFadeOrSweep">使用渐变换页还是平移换页？</param>
    public void MenuP_PageIndex(int INDEX, bool useFadeOrSweep = false)
    {
        if(INDEX == iCurrentPageIndex) return;

        if(isSwitchingPage == true) return;// 本次翻页完成之前不许翻到其他页
        else isSwitchingPage = true;

        if (INDEX > CanvasList.Count - 1)
        {
            Debug.LogAssertion("欲翻的页的索引超出CanvasList大小，请考虑是否少页。");
            return;
        }

        //如果从视频页换走 ，就停止播放视频并恢复导航栏的显示
        if (iCurrentPageIndex == 5 && INDEX != 5 && VP != null)
        {
            VP.Stop();
            CONTROL_ToggleNavBar(true);
        }
 
        iLastPageIndex = iCurrentPageIndex;//更新上一页页码索引
        iCurrentPageIndex = INDEX;//更新页码索引

            //!将水平过渡动画页加入此列表以解决返回时使用渐变过渡动画页的BUG
            switch(iLastPageIndex)
            {
                case 5: useFadeOrSweep = true; reverseSweepDirOnce = true; break;
                case 6: useFadeOrSweep = true; reverseSweepDirOnce = true;  break;
                case 7: useFadeOrSweep = true; reverseSweepDirOnce = true;  break;
                case 8: useFadeOrSweep = true; reverseSweepDirOnce = true;  break;
                case 9: useFadeOrSweep = true; reverseSweepDirOnce = true;  break;
                case 10: useFadeOrSweep = true; reverseSweepDirOnce = false;  break;
                case 11: useFadeOrSweep = true; reverseSweepDirOnce = false;  break;
                case 12: useFadeOrSweep = true; reverseSweepDirOnce = true;  break;
                default:break;
            }
        
        if(useFadeOrSweep == true) teleport = true;//将当前页面瞬移1080个单位并由协程搬回来

        if (INDEX < 3)//只在index为0、1、2时才播放任务栏动画
        {
            AnimCtrl.SetInteger("index", INDEX);//播放指标动画
            NAVBar_Animator.SetInteger("index", INDEX);//播放图标动画
        }
        
        //主菜单三页要一直显示导航栏
        if(iCurrentPageIndex <= 2 ) CONTROL_ToggleNavBar(true);

        //细节页观看视频时可以点两下，点第二下会横屏，不应该空视频
        if(iLastPageIndex == 5 && iCurrentPageIndex != 12) 
        {
            VP.url = VideoNullUrl;
        }

        //如果要换到的页是非视频页，就停止播放
        if(iCurrentPageIndex != 5)
        {
            if(iCurrentPageIndex != 12)
            {
                VP.url = VideoNullUrl;
            }
        }

        //确保在视频播放页是横屏、其他页是竖屏
        if(iCurrentPageIndex == 5) APIManager.API.API_RotateScreen(true);
        else  APIManager.API.API_RotateScreen(false);
        
        if(iCurrentPageIndex == 10) iLastPageIndex = 0;//在三级目录时防止无法返回上上一页，这里需要更好的返回页办法。
        if(iCurrentPageIndex == 11) iLastPageIndex = 10;//在三级目录时防止无法返回上上一页，这里需要更好的返回页办法。
        if(iCurrentPageIndex == 12) iLastPageIndex = 11 ;//防止全屏观看退不出去

    }

    /// <summary>
    /// 删除全部下载内容。
    /// </summary>
    public void CONTROL_ClearDLC()
    {
        string path = Application.persistentDataPath + "/" + "DownloadableContent";
        if (Directory.Exists(path) == false) return;//查看目录是否存在，如果不存在就完事
        var dirSource = new DirectoryInfo(path);    //获取目录信息
        foreach (FileInfo FI in dirSource.GetFiles())    //从目录信息中读取文件信息
        {
            File.Delete(FI.FullName);
        }

    }

    /// <summary>
    /// 切换隐藏显示导航栏
    /// </summary>
    public void CONTROL_ToggleNavBar()
    {
        NAVBar.GetComponent<Canvas>().enabled = !NAVBar.GetComponent<Canvas>().enabled;
    }

    /// <summary>
    /// 显示或隐藏导航栏
    /// </summary>
    /// <param name="Visible"></param>
    public void CONTROL_ToggleNavBar(bool Visible)
    {
        NAVBar.GetComponent<Canvas>().enabled = Visible;
    }











    /// <summary>
    /// 返回上一页
    /// </summary>
    public void CONTROL_BackPage()
    {

        // !将所有换页函数按顺序加在这里
        // !否则将无法返回页面
        switch (iLastPageIndex)
        {
            case 0: CONTROL_GoToHandPage(); break;
            case 1: CONTROL_GoToLearnPage(); break;
            case 2: CONTROL_GoToMePage(); break;
            case 3: CONTROL_GoToLoginPage(); break;
            case 4: CONTROL_GoToSigninPage(); break;
            case 5: CONTROL_GoToVideoPlayPage(); break;
            case 6: CONTROL_C_MotdPage(); break;
            case 7: CONTROL_C_AudioPage(); break;
            case 8: CONTROL_C_ActivePage(); break;
            case 9: CONTROL_C_DaKaPage(); break;
            case 10: CONTROL_C_VideoClassPage(); break;
            case 11: CONTROL_C_VideoSelectPage(); break;
            case 12: CONTROL_C_VideoDetailPage(); break;
            default: Debug.LogError("未注册该返回页！"); break;
        }

    }

    public void CONTROL_GoToHandPage()
    {
        MenuP_PageIndex(0);
        ToggleNavHeadLightMode(1);
        
    }

    public void CONTROL_GoToLearnPage()
    {
        MenuP_PageIndex(1);
        ToggleNavHeadLightMode(1);
    }

    public void CONTROL_GoToMePage()
    {
        MenuP_PageIndex(2);
        ToggleNavHeadLightMode(0);
    }

    public void CONTROL_GoToLoginPage()
    {
        /* 如果现在已经登录，就退出登录 */
        if (UserManager.isLogin == true)
        {
            SendMessage1(true,"成功退出登录。");
            UserManager.Logout();
        }
        else
        {
            //如果现在没有登录，点击后就会跳转到登录页面
            CONTROL_ToggleNavBar(false);//登录页隐藏导航栏
            MenuP_PageIndex(3);
        }
    }

    public void CONTROL_GoToSigninPage()
    {
        MenuP_PageIndex(4);
        ToggleNavHeadLightMode(1);
    }

    public void CONTROL_GoToVideoPlayPage(string URL, string Author, string Describe, int PageIndex = 5)
    {   
        if (VP == null) return;

        //TODO:判断当前是否有网
        
        VP_Author.text = Author;
        VP_Describe.text = Describe;
        MenuP_PageIndex(PageIndex, true);
        ToggleNavHeadLightMode(0);
        CONTROL_ToggleNavBar(false);
        VP.url = VideoNullUrl;
        VP.Play();
        SendMessage1(true, "正在下载视频", 120f);
        APIManager.API.DL_StartDownload(URL, Callback_DownloadForDownloadVideo);
    }
    private void Callback_DownloadForDownloadVideo(bool isOK, string FileFullPath)
    {
        Invoke("ClearMessage", 2f);
        
        if(isOK == false)
        {
            SendMessage1(false, "视频下载失败");
            if(iCurrentPageIndex == 5 || iCurrentPageIndex == 12) CONTROL_BackPage();//如果视频下载失败就返回上一页
            return;
        }

        SendMessage1(true, "视频下载完成");
        VP.url = FileFullPath;
        VP.Play();
    }

    public void CONTROL_GoToVideoPlayPage()//用于页面后退功能
    {   
        if (VP == null) return;

        MenuP_PageIndex(5);
        ToggleNavHeadLightMode(0);
        CONTROL_ToggleNavBar(false);
        VP.Play();
    }

    public void CONTROL_C_MotdPage()
    {
        CONTROL_ToggleNavBar(false);
        MenuP_PageIndex(6, true);
    }

    public void CONTROL_C_AudioPage()
    {
        CONTROL_ToggleNavBar(false);
        MenuP_PageIndex(7, true);
    }

    public void CONTROL_C_ActivePage()
    {
        CONTROL_ToggleNavBar(false);
        MenuP_PageIndex(8, true);
    }

    public void CONTROL_C_DaKaPage()
    {
        CONTROL_ToggleNavBar(false);
        MenuP_PageIndex(9, true);
    }

    public void CONTROL_C_VideoClassPage()
    {
        CONTROL_ToggleNavBar(false);
        MenuP_PageIndex(10, true);
    }

    public void CONTROL_C_VideoSelectPage()
    {
        CONTROL_ToggleNavBar(false);
        MenuP_PageIndex(11, true);
        VP.url = VideoNullUrl;//从视频详细页退出，停止视频
    }

    public void CONTROL_C_VideoDetailPage(string URL,string Title,string Describe)
    {
        CONTROL_ToggleNavBar(false);
        ToggleNavHeadLightMode(1);
        FindObjectOfType<SRP_videoDetail_Manager>().Init(URL, Title, Describe);
    }
    public void CONTROL_C_VideoDetailPage()
    {
        if (VP == null) return;

        MenuP_PageIndex(12);
        ToggleNavHeadLightMode(1);
        CONTROL_ToggleNavBar(false);
        VP.Play();
    }

    





    //导航栏白天、夜间模式切换
    bool isNightMode = false;
    /// <summary>
    /// 切换状态栏夜间日间模式
    /// 参数填1是夜间模式，参数0是阳间模式，参数-1是切换模式(比如现在是夜间那就换阳间)
    /// </summary>
    /// <param name="NightMode"></param>
    public void ToggleNavHeadLightMode(int NightMode = -1)
    {
        if (NAVHead == null) return;

        if (NightMode == -1)
        {
            if (isNightMode)
            {
                NAVHead.GetComponent<SRP_NavHead>().Battery.color = Color.white;
                NAVHead.GetComponent<SRP_NavHead>().Server.color = Color.white;
                NAVHead.GetComponent<SRP_NavHead>().Wifi.color = Color.white;
                NAVHead.GetComponent<SRP_NavHead>().TEXT.color = Color.white;

            }
            else
            {
                NAVHead.GetComponent<SRP_NavHead>().Battery.color = Color.black;
                NAVHead.GetComponent<SRP_NavHead>().Server.color = Color.black;
                NAVHead.GetComponent<SRP_NavHead>().Wifi.color = Color.black;
                NAVHead.GetComponent<SRP_NavHead>().TEXT.color = Color.black;
            }
            isNightMode = !isNightMode;
        }
        if (NightMode == 0)
        {
            NAVHead.GetComponent<SRP_NavHead>().Battery.color = Color.white;
            NAVHead.GetComponent<SRP_NavHead>().Server.color = Color.white;
            NAVHead.GetComponent<SRP_NavHead>().Wifi.color = Color.white;
            NAVHead.GetComponent<SRP_NavHead>().TEXT.color = Color.white;
        }
        if (NightMode == 1)
        {
            NAVHead.GetComponent<SRP_NavHead>().Battery.color = Color.black;
            NAVHead.GetComponent<SRP_NavHead>().Server.color = Color.black;
            NAVHead.GetComponent<SRP_NavHead>().Wifi.color = Color.black;
            NAVHead.GetComponent<SRP_NavHead>().TEXT.color = Color.black;
        }
    }

    /// <summary>
    /// 跳转到序号对应Collection的横屏主菜单
    /// </summary>
    public void CONTROL_GoToCollection_01()
    {
        if(UserManager.isLogin == false)
        {
            SendMessage1(false,"请先登录。");
            print("未登录账号，禁止操作。");
            return;
        }
        else
        {
            print("正在跳转至横屏页面..");
            APIManager.API.API_BackToMenu(false);
        }

    }
    public void CONTROL_GoToCollection_02()
    {
        //APIManager.API.API_LessonIndexCollection_Set(1);
        APIManager.API.API_BackToMenu(false);
    }
    public void CONTROL_GoToCollection_03()
    {
        // APIManager.API.API_LessonIndexCollection_Set(2);
        // APIManager.API.API_BackToMenu(false);
        APIManager.API.API_LoadScene_SetName("SE_shPaint");
        APIManager.API.API_LoadScene();
    }
    public void CONTROL_GoToCollection_04()
    {
        //APIManager.API.API_LessonIndexCollection_Set(3);
        APIManager.API.API_BackToMenu(false);
    }

    /// <summary>
    /// 新版渐变换页
    /// </summary>
    /// <returns></returns>
    bool doOnce = false;
    IEnumerator ChangePageInFade()
    {
        int index;
        float Speed_Pos = 0.1f;
        List<Canvas> C_List = new List<Canvas>();
        List<CanvasGroup> CG_List = new List<CanvasGroup>();

        foreach(GameObject GO in CanvasList) 
        {
            C_List.Add(GO.GetComponent<Canvas>());
            CG_List.Add(GO.GetComponent<CanvasGroup>());
        }

        while(true)
        {
            foreach(CanvasGroup CG in CG_List)
            {
                index = CG_List.IndexOf(CG);
                Speed_Pos = Time.deltaTime * 10f;

                //设置透明度
                if(index != MenuP_PageIndex()) CG.alpha -= Speed_Pos;
                else CG.alpha += Speed_Pos;

                //设置控制  如果直接 enable = false 会直接消失，所以要等透明度近乎不透明的时候再false
                if(CG.alpha <= 0.05f) C_List[index].enabled = false;
                else C_List[index].enabled = true;
            }

            if(CG_List[iCurrentPageIndex].alpha >= 0.99f)
            {
                if(doOnce == false) 
                {
                    doOnce = true;
                    ChangePageCallback(false);
                }
            }
            else doOnce = false;

            yield return null;
        }
    }

    bool teleport = false;//水平换页的开关
    bool doOnce2 = false;//确保回调函数只能调用一次
    IEnumerator ChangePageInSweep()
    {
        const float Speed_Pos = 5000f;
        const float Speed_Scale = 5f;
        Vector3 pastPageTargetScale = new Vector3(1f,1f,1f);//上一页的目标大小，所有页面的Scale默认都应当是1
        Vector2 pastPageTargetLoc = new Vector3(0f,0f,0f);

        while(true)
        {
            //更新当前页位置
            Vector2 newPos = Vector2.MoveTowards(CanvasTransformList[iCurrentPageIndex].anchoredPosition, new Vector2(0f, 0f), Speed_Pos*Time.deltaTime);
            CanvasTransformList[iCurrentPageIndex].anchoredPosition = newPos;

            //更新老页面位置和大小
            if(iLastPageIndex != -1)
            {
                Vector3 newScale = Vector3.MoveTowards(CanvasTransformList[iLastPageIndex].localScale, pastPageTargetScale, Speed_Scale*Time.deltaTime);
                CanvasTransformList[iLastPageIndex].localScale = newScale;
            
                Vector2 newPos2 = Vector2.MoveTowards(CanvasTransformList[iLastPageIndex].anchoredPosition, pastPageTargetLoc, Speed_Pos*Time.deltaTime);
                CanvasTransformList[iLastPageIndex].anchoredPosition = newPos2;
            }



            if(teleport == true)//瞬移位置
            {
                teleport = !teleport;//只瞬移一次
                CanvasList[iCurrentPageIndex].GetComponent<Canvas>().enabled = true;
                CanvasList[iCurrentPageIndex].GetComponent<CanvasGroup>().alpha = 1f;
                CanvasList[iCurrentPageIndex].GetComponent<Canvas>().sortingOrder = 1;//默认所有页都是0，现在把要换到的页设为1，就可以盖住任何一页了
                if(reverseSweepDirOnce == false) CanvasTransformList[iCurrentPageIndex].anchoredPosition = new Vector2(1080f,0f);
                else CanvasTransformList[iCurrentPageIndex].anchoredPosition = new Vector2(-1080f,0f);
                pastPageTargetScale = new Vector3(0.1f,0.1f,0.1f);//设置老页目标大小
                //if(iLastPageIndex != -1) CanvasTransformList[iLastPageIndex].GetComponent<Image>().color = new Color(0f,0f,0f, 0.9f);//将老页面设成半透黑色，特效已删除
                if(reverseSweepDirOnce == false) pastPageTargetLoc = new Vector2(-1080f,0);
                else pastPageTargetLoc = new Vector2(1080f,0);
            }

            //如果移动完成
            if (GetVector2Length(CanvasTransformList[iCurrentPageIndex].anchoredPosition) <= 1f)
            {
                if(doOnce2 == false)
                {
                    doOnce2 = true;
                    if(iLastPageIndex != -1) CanvasList[iLastPageIndex].GetComponent<Canvas>().enabled = false;//将上一页隐藏
                    CanvasList[iCurrentPageIndex].GetComponent<Canvas>().sortingOrder = 0;//重置当前页的顺序为原先顺序
                    CanvasTransformList[iCurrentPageIndex].anchoredPosition = new Vector2(0f, 0f);//修正位置0
                    pastPageTargetScale = new Vector3(1f, 1f, 1f);//初始化老页目标大小和老页当前大小
                    if(iLastPageIndex != -1) CanvasTransformList[iLastPageIndex].localScale = pastPageTargetScale;
                    pastPageTargetLoc = new Vector2(0f, 0f);//初始化老页目标大小和老页当前大小
                    if(iLastPageIndex != -1) CanvasTransformList[iLastPageIndex].anchoredPosition = pastPageTargetLoc;
                    if(iLastPageIndex != -1) CanvasTransformList[iLastPageIndex].GetComponent<Image>().color = new Color(0f,0f,0f, 0f);//初始化老页面颜色
                    ChangePageCallback(true);
                }
            }
            else doOnce2 = false;

            yield return null;
        }
    }

    //翻页回调函数，两个翻页功能最终都会聚集到这里
    private void ChangePageCallback(bool isSweep)
    {
        //print(APIManager.API.Selector("横屏切换完成","渐变切换完成",isSweep));
        reverseSweepDirOnce = false;//重置该变量
        isSwitchingPage = false;
    }

    public void CONTROL_FunctionNotReady()
    {
        SendMessage1(false, "该功能已在开发");
    }


    public void SendMessage1(bool isOK, string Message, float LifeTime = 2.0f)
    {
        if(SRP_MessageSystem.MS == null)
        {
            Debug.LogError("未检测到MessageSystem，无法Send。");
            return;
        }
        SRP_MessageSystem.MS.SendMessage(isOK,Message,LifeTime);
    }
    public void ClearMessage() => SRP_MessageSystem.MS.ClearAllMessage();

    private float GetVector2Length(Vector2 VEC)
    {
        return Mathf.Sqrt(Mathf.Pow((VEC.x), 0.5f) + Mathf.Pow((VEC.y), 0.5f));
    }
}
