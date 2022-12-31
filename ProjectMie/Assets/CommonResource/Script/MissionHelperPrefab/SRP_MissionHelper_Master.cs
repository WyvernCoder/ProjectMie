using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_MissionHelper_Master : MonoBehaviour
{
    [Header("下一关卡的ID，用于解锁关卡。")]
    public string NextSceneID = "";
    [Header("下一关卡的名称，用于载入关卡，留空即返回。")]
    public string NextSceneName = "";
    public List<Container> MissionClassList = new List<Container>();
    [Space(10)]// !添加新模块需要在这里注册
    public SRP_MH_Context ContextPrefab;
    public SRP_MH_Draw DrawPrefab;
    public SRP_MH_Listen ListenPrefab;
    public SRP_MH_SelectAndClear SelectAndClearPrefab;
    public SRP_MH_Custom CustomPrefab;

    [HideInInspector]
    public int CurrentMissionIndex = 0;
    [HideInInspector]
    public GameObject CurrentMissionGameObject;
    [HideInInspector]
    public SRP_MH_Master LastMission;
    [HideInInspector]
    public SRP_MH_Master NowMission;

    void Awake()
    {
        APIManager.GENERATE_BODY();
    }

    void Start()
    {
        BeginMission();
    }

    public void BeginMission()
    {
        //print("任务开始！");
        CurrentMissionIndex--;
        PlayNext(true);
    }

    public void PlayNext(bool isBegin = false)
    {
        if(isBegin == false)
        LastMission = CurrentMissionGameObject.GetComponent<SRP_MH_Master>();

        CurrentMissionIndex++;

        if(CurrentMissionIndex == MissionClassList.Count)
        {
            print("全部任务已经完成。");
            if(APIManager.API != null) 
            {
                 /* 解锁下一关 */
                if(NextSceneID != "") UserManager.SetUserSubscribeClassBool(NextSceneID, true);
                
                if(NextSceneName != "")
                {
                    /* 读取下一关 */
                    APIManager.API.API_LoadScene_SetName(NextSceneName);
                    APIManager.API.API_LoadScene();
                }
                else APIManager.API.API_SceneGoBack(true);
            }
            
            Destroy(CurrentMissionGameObject);
            if(LastMission != null) Destroy(LastMission);
            Destroy(gameObject);
            return;
        }

        switch(MissionClassList[CurrentMissionIndex].type)
        {   //!如了新的阶段模块，则需要在此处注册
            case Container.MissionType.Context: PlayContext(); break;
            case Container.MissionType.Draw: PlayDraw(); break;
            case Container.MissionType.Listen: PlayListen(); break;
            case Container.MissionType.SelectAC: PlaySelectAC(); break;
            case Container.MissionType.Custom: PlayCustom(); break;
        }

        NowMission = CurrentMissionGameObject.GetComponent<SRP_MH_Master>();

        //开始渐变动画
        if(isBegin == false)
        StartCoroutine(PlayNext_Anim());
    }

    IEnumerator PlayNext_Anim()
    {
        //防止开始另一段动画后，上一段动画没人管
        //所以每个协程都要记住自己负责的任务
        SRP_MH_Master LastMissionCache = LastMission;
        SRP_MH_Master NowMissionCache = NowMission;
        float Speed = 5f * Time.deltaTime;

        //初始化透明度
        NowMissionCache.CG.alpha = 0;
        LastMissionCache.CG.alpha = 1;

        while(true)
        {
            NowMissionCache.CG.alpha += Speed;
            LastMissionCache.CG.alpha -= Speed;
            if(NowMissionCache.CG.alpha >= 0.98f) break;
            yield return null;
        }

        NowMissionCache.CG.alpha = 1;
        Destroy(LastMissionCache.gameObject);

        yield break;
    }

    public object PlayContext()
    {
        CurrentMissionGameObject = Instantiate(ContextPrefab.gameObject);
        CurrentMissionGameObject.GetComponent<SRP_MH_Context>().Initial(MissionClassList[CurrentMissionIndex].context, this);
        return MissionClassList[CurrentMissionIndex].context;
    }

    
    public object PlayDraw()
    {
        CurrentMissionGameObject = Instantiate(DrawPrefab.gameObject);
        CurrentMissionGameObject.GetComponent<SRP_MH_Draw>().Initial(MissionClassList[CurrentMissionIndex].draw, this);
        return MissionClassList[CurrentMissionIndex].draw;
    }

    
    public object PlayListen()
    {
        CurrentMissionGameObject = Instantiate(ListenPrefab.gameObject);
        CurrentMissionGameObject.GetComponent<SRP_MH_Listen>().Initial(MissionClassList[CurrentMissionIndex].listen, this);
        return MissionClassList[CurrentMissionIndex].listen;
    }

    
    public object PlaySelectAC()
    {
        CurrentMissionGameObject = Instantiate(SelectAndClearPrefab.gameObject);
        CurrentMissionGameObject.GetComponent<SRP_MH_SelectAndClear>().Initial(MissionClassList[CurrentMissionIndex].selectAndClear, this);
        return MissionClassList[CurrentMissionIndex].selectAndClear;
    }

    public object PlayCustom()
    {   
        CurrentMissionGameObject = Instantiate(CustomPrefab.gameObject);
        CurrentMissionGameObject.GetComponent<SRP_MH_Custom>().Initial(MissionClassList[CurrentMissionIndex].custom, this);
        return MissionClassList[CurrentMissionIndex].custom;
    }
}



[System.Serializable]
public class Container
{
    // !若添加了新模块则需要在这里注册
    public enum MissionType {Context, Draw, SelectAC, Listen, Custom}
    public MissionType type;
    public Context context;
    public Draw draw;
    public SelectAndClear selectAndClear;
    public Listen listen;
    public Custom custom;
} 
[System.Serializable]
public class Context
{   // *对话模块
    [Header("对话段落")]
    public List<string> Paragraph = new List<string>();

    [Header("人物图像")]
    [Space(10)]
    public List<Sprite> LeftCharacterImage = new List<Sprite>();
    public List<Sprite> RightCharacterImage = new List<Sprite>();

    [Header("背景图像")]
    [Space(10)]
    public Sprite BGImage;
    
    [Header("声音设置")]
    [Space(10)]
    public List<AudioClip> Voice = new List<AudioClip>();
    public AudioClip Music = null;
    public AudioClip WordPop = null;
}

[System.Serializable]
public class Draw
{   // *画画模块
    [Header("背景图片设置")]
    public Sprite BGImage;

    [Header("画画任务设置")]
    [Space(10)]
    public List<SRP_LineDrawerWithMission> MissionList = new List<SRP_LineDrawerWithMission>();

    [Header("声音设置")]
    [Space(10)]
    public AudioClip Music = null;

}

[System.Serializable]
public class SelectAndClear
{   // *消消乐模块
    [Header("背景图片设置")]
    public Sprite BGImage;

    [Header("消消乐图片设置")]
    [Space(10)]
    public List<Sprite> ImageCollection = new List<Sprite>();
    public Vector2 ImageSize = new Vector2(512f, 512f);
    public float ImageSpace = 10f;

    [Header("声音设置")]
    [Space(10)]
    public AudioClip Music;
    public List<AudioClip> SoundCollection = new List<AudioClip>();
}

[System.Serializable]
public class Listen
{   // *听字模块
    [Header("背景图设置")]
    public Sprite BGImage;
    
    [Header("文字设置")]
    [Space(10)]
    public string Word = "天";
    public string PinYin = "tiān";
    
    [Header("声音设置")]
    [Space(10)]
    public AudioClip Music;
    public AudioClip Voice;
}

[System.Serializable]
public class Custom
{   // *自定义任务模块
    [Header("自定义Prefab设置")]
    public GameObject PrefabToSpawn;
}
