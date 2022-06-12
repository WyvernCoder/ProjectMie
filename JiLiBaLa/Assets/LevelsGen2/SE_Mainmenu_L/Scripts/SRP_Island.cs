using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SRP_Island : MonoBehaviour
{
    //TODO在吗，如果你看到了这条，就意味着优化的任务...
    //!交给你了！！！！
    //*很辛苦吧，记得少用GetComponent，很kill性能的！

    /// <summary>
    /// 序列化参数部分
    /// 可在此调整序列化多边形关卡数量及多边形关卡内菜单
    /// 和多边形关卡间距之类的
    /// </summary>
    
    [Tooltip("该Island在数据库ClassData中对应的Index，每个Island都应具有独一无二的Index。")]
    public int IslandIndex = -1;

    [Header("程序化SubIsland设置")]
    [Tooltip("启动自动刷新显示功能，仅影响编辑器，不会影响游戏性。")]
    public bool Enable = true;
    [Tooltip("关卡元素间距")]
    public float SpaceBetweenShapeLevel = 3f;
    [Tooltip("整体偏移X")]
    public float OffsetX = -5f;
    [Tooltip("整体偏移Y")]
    public float OffsetY = 0f;
    [Tooltip("峰值")]
    public float SinA = 1f;
    [Tooltip("周期")]
    public float SinOmega = 1f;
    [Tooltip("波形在X轴位置偏移")]
    public float SinTheta = 1f;
    [Tooltip("波形在Y轴位置偏移")]
    public float SinB = 1f;


    public List<SubIslandFormat> SubIslandSetting = new List<SubIslandFormat>();

    [System.Serializable]
    public class SubIslandFormat
    {
        [HideInInspector]
        public int ShapeLevelIndex = -1;//多边形关卡的index

        [Header("多边形关卡设置")]
        
        [Tooltip("多边形关卡边框图片")]
        public Sprite PopImage = null;
        
        [Tooltip("多边形关卡边框蒙版")]
        public Sprite MaskImage = null;

        [Tooltip("多边形关卡图标")]
        public Sprite Icon = null;
        
        [Tooltip("多边形关卡名图片")]
        public Sprite TitleImage = null;

        [Tooltip("SubMainmenu的背景图片")]
        public Sprite SubMainmenuBGImage = null;

        [Header("子目录设置")]

        [Tooltip("Level间距")]//TODO
        public float LevelSpace = 148.3f;

        public List<LevelFormat> LevelSetting = new List<LevelFormat>();

    }

    [System.Serializable]
    public class LevelFormat
    {
        [HideInInspector]
        public int LevelIndex = -1;

        [Tooltip("在子菜单中的关卡标题")]
        public string Title = "1-1 Pander杀手";
        [Tooltip("关卡Prefab的引用")]
        public GameObject Prefab = null;
        [Tooltip("胜利Prefab的引用")]
        public GameObject WinPrefab = null;
        [Tooltip("失败Prefab的引用")]
        public GameObject FailedPrefab = null;
        [Tooltip("在子菜单中的图标")]
        public Sprite Icon = null;
        [Tooltip("多边形关卡图标缩放")]
        public Vector2 Scale = new Vector2(1f,1f);

    }

    [HideInInspector]
    public SRP_TouchMover TouchMover;//手指移动器，用于停止手指移动屏幕的功能

    [HideInInspector]
    public SRP_Control MainmenuController;//界面控制器，用于控制返回按钮的作用，比如是关闭当前窗口还是返回主菜单

    public GameObject SubMainmenuObject;
    public GameObject ShapeObject;
    public GameObject LevelObject;
    
    [HideInInspector]
    public List<GameObject> LevelObjectList = new List<GameObject>();
    
    [HideInInspector]
    public List<GameObject> SubMainmenuObjectList = new List<GameObject>();

    [HideInInspector]
    public string ABFullPath;
    
    private Vector3 newLocation = new Vector3();

    void Awake()
    {
        if(Enable) Enable = false;
    }

    void Start()
    {
        TouchMover = GameObject.Find("PRB_TouchMover").gameObject.GetComponent<SRP_TouchMover>();
        if(TouchMover == null) print("未找到触控移动器！");
        MainmenuController = GameObject.Find("Canvas_Control").gameObject.GetComponent<SRP_Control>();
        if(MainmenuController == null) print("未找到界面控制器！");

        //生成并初始化SubMainmenu和SubIsland
        if(Application.isPlaying == true) StartCoroutine(SpawnSubLevel_IE());
        StartCoroutine(SpawnSubMainmenu_IE());
        

        //进入数据库的世界！
        //TODO:测试用，应在主菜单连接数据库
        WalkIntoDatabase();
    }


    void Update()
    {
        if(Application.isPlaying == false && Enable) StartCoroutine(SpawnSubLevel_IE());
    }
    //TODO：我需要：
    //结构：    ClassData中元素为Island类                                       ok
    //          Island类中有SMUnlock布尔数组以表示其解锁情况                    ok
    //          Island类中有Score整形数组表示每个SubMainmenu的得分              ok
    //          Island类中有Level类                                             ok
    //              Level类中有Score整形数组以表示其得分情况                        ok
    //              Level类中有LUnlock布尔数组表示其解锁情况                    ok
    //变量1：SRP_Island中添加ID标识符用以标记自己对应数据库ClassData的元素          ok
    //函数1：根据Island的ID去动态添加数据库Island
    //函数2：使Level检测自身是否被解锁
    //函数3：解锁下一关
    //函数4：判断下一关是否被安装
    //函数5：判断Island的index是否被占用

    public void WalkIntoDatabase() => StartCoroutine(WalkIntoDatabase_());
    IEnumerator WalkIntoDatabase_() //!需有数据库连接
    {   //该协程用于确保Island对应的数据库信息存在
        //*也作为Island接入数据库的大门。

        //等待其他东西载入完毕
        yield return null;

        //检测是否存在用户的数据库信息
        if(SRP_DatabaseManager.Instance.UserAccount.sName == null)
        {
            //编辑器模式下自动登录账号
            if(Application.isEditor)
            {
                SRP_DatabaseManager.Instance.UserData_Download("test");
                yield return null;

                //如果没有测试账号，就注册一个
                if(SRP_DatabaseManager.Instance.UserAccount.sName == null) 
                {
                    SRP_DatabaseManager.Instance.UserData_Create("test","test","test","test");
                    yield return null;
                    SRP_DatabaseManager.Instance.UserData_Download("test");
                    yield return null;
                    if(SRP_DatabaseManager.Instance.UserAccount.sName == null) 
                    {
                        print("开不了账号，别测了，我帮你把协程给扬了。");
                        yield break;
                    }
                }
            }
            else
            {
                print("未能读取数据库信息，联网协程中断。");//TODO:从此进入离线模式
                yield break;
            }
        }

        //数据库信息不够时就自动填补ClassData
        if(SRP_DatabaseManager.Instance.UserAccount.ClassData.Count - 1 < IslandIndex)
        {
            while(SRP_DatabaseManager.Instance.UserAccount.ClassData.Count - 1 < IslandIndex)
            {
                SRP_DatabaseManager.Instance.UserAccount.ClassData.Add(new SRP_DatabaseManager.Island());
                yield return null;
            }
            SRP_DatabaseManager.Instance.UserData_Upload(SRP_DatabaseManager.Instance.UserAccount.sPhoneNum);
        }
        //TODO:LevelObjectList
        //TODO:SubMainmenuObjectList

        //TODO:解锁或锁定SubIsland
        // foreach(var GO in LevelObjectList)
        // {
        //     int index = LevelObjectList.IndexOf(GO);
        //     if(SRP_DatabaseManager.Instance.UserAccount.ClassData[IslandIndex].SMUnlock[index] == false) GO.GetComponent<BoxCollider2D>().enabled = false;
        // }

        yield break;
    }

    IEnumerator SpawnSubLevel_IE()//!创建SubIsland
    {
        newLocation = new Vector3(gameObject.transform.position.x + OffsetX + SpaceBetweenShapeLevel, gameObject.transform.position.y + OffsetY, gameObject.transform.position.z);
        LevelObjectList.Clear();
        foreach (SubIslandFormat SIF in SubIslandSetting)
        {
            int index = SubIslandSetting.IndexOf(SIF);
            var GO = Instantiate(ShapeObject);//!GO是SubIsland
            GO.transform.SetParent(gameObject.transform.Find("SubIslands/").transform,true);
            GO.GetComponent<SRP_SubIsland>().IslandBase = this;
            GO.GetComponent<SRP_SubIsland>().subIslandIndex = index;
            if(Application.isEditor && Enable) GO.GetComponent<SRP_SubIsland>().EnableAutoClean = true;//实时预览功能需要搭配自动清理功能使用，否则会不断刷新
            GO.transform.Find("IconMask/Icon").GetComponent<SpriteRenderer>().sprite = SIF.Icon;
            GO.transform.Find("TitleSprite").GetComponent<SpriteRenderer>().sprite = SIF.TitleImage;
            GO.GetComponent<SpriteRenderer>().sprite = SIF.PopImage;
            GO.transform.Find("IconMask/").GetComponent<SpriteMask>().sprite = SIF.MaskImage;
            GO.transform.position = new Vector3(newLocation.x + SpaceBetweenShapeLevel, newLocation.y  + Mathf.Sin(SinOmega * (newLocation.x + SpaceBetweenShapeLevel) + SinTheta) * SinA + SinB, newLocation.z);
            newLocation = GO.transform.position;
            LevelObjectList.Add(GO);
        }
        yield break;
    }

    IEnumerator SpawnSubMainmenu_IE()//!创建SubMainmenu
    {
        if(Application.isPlaying == false) yield break;//编辑器模式下不生成UI，可视性太差
        foreach(SubIslandFormat SIF in SubIslandSetting)//遍历SubIsland
        {
            var SMO = Instantiate(SubMainmenuObject);//SMO是SubMainmenu，有几个SubIsland就刷几个SubMainmenu
            SMO.transform.Find("Background/").GetComponent<Image>().sprite = SIF.SubMainmenuBGImage;
            SMO.transform.SetParent(gameObject.transform.Find("Submainmenus/"));
            SMO.GetComponent<SRP_SubMainmenu>().Island = this;
            SMO.GetComponent<SRP_SubMainmenu>().SubMainmenuIndex = SubIslandSetting.IndexOf(SIF);
            SMO.transform.Find("Scroll View/Viewport/Content").GetComponent<HorizontalLayoutGroup>().spacing = SIF.LevelSpace;//设置SubMainmenu里小关卡的间距
            foreach(LevelFormat LF in SIF.LevelSetting)//遍历SubIsland里的LevelSetting
            {   //!GO是Level
                var GO = Instantiate(LevelObject);
                GO.GetComponent<SRP_Level>().SubMainmenu = SMO.GetComponent<SRP_SubMainmenu>();
                GO.GetComponent<SRP_Level>().LevelIndex = SIF.LevelSetting.IndexOf(LF);
                GO.GetComponent<SRP_Level>().Prefab = LF.Prefab;
                GO.GetComponent<SRP_Level>().WinPrefab = LF.WinPrefab;
                GO.GetComponent<SRP_Level>().FailedPrefab = LF.FailedPrefab;
                GO.GetComponent<SRP_Level>().MyIsland = this;
                GO.GetComponent<Image>().sprite = LF.Icon;
                GO.transform.Find("TitleText/").GetComponent<Text>().text = LF.Title;
                GO.transform.localScale = LF.Scale;
                GO.transform.SetParent(SMO.transform.Find("Scroll View/Viewport/Content/").transform);
            }
            SubMainmenuObjectList.Add(SMO);
        }

        //隐藏菜单
        foreach(var GO in SubMainmenuObjectList) GO.GetComponent<Canvas>().enabled = false;

        yield break;
    }
}