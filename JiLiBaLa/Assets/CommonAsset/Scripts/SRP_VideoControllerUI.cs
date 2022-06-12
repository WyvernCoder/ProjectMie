using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
/// <summary>
/// SRP_VideoControllerUI
/// 作用：提供视频播放所需的功能。
/// 用法：
/// </summary>

public class SRP_VideoControllerUI : MonoBehaviour
{
    [Header("对象引用部分")]

    [Tooltip("包含Button组件的GameObject，用于控制视频的播放与暂停。")]
    public GameObject ButtonObject;

    [Tooltip("包含Image组件的GameObject，用于显示播放/暂停的图片。")]
    public GameObject StatueObject;
    
    [Tooltip("包含VideoPlayer组件的GameObject，用于播放视频。")]
    public GameObject VideoPlayerObject;

    [Tooltip("包含Slider组件的GameObject，起到视频进度条的作用。")]
    public GameObject SliderObject;

    [Tooltip("播放速度Text。")]
    public Text PlayspeedText;

    [HideInInspector,Tooltip("VideoPlayer会将视频画面投影到该Camera上，Camera会赋值为Find方法搜寻到的第一个名为Main Camera的相机，不需要用户手动赋值调整。")]
    public Camera CameraObject;

    


    [Header("用户赋值部分")]

    [Tooltip("播放按钮Sprite图标。")]
    public Sprite Image_Play;
    
    [Tooltip("暂停按钮Sprite图标。")]
    public Sprite Image_Pause;

    [Tooltip("需要播放的视频。")]
    public VideoClip VideoToPlay;

    [Tooltip("当进度条进度大于此值，就可以解锁下一关卡。")]
    public float TargetProcess = 0.8f;

    [Tooltip("点击左上角返回主菜单是否可以解锁下一关卡")]
    public bool BackToMenuCanUnlockNextScene = true;

    void Awake()
    {
        if(VideoToPlay == null)//自我检测，避免不必要的计算
        {
            print("找不到需要播放的视频。");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(FindCameraAndPlay());//开启协程寻找Main Camera，如果这里直接使用while去寻找相机，会卡死Unity并降低运行效率。
    }

    void FixedUpdate()
    {
        //为Slider的value赋值为：当前时间➗总时间，即：播放进度。
        SliderObject.GetComponent<Slider>().value = (float)VideoPlayerObject.GetComponent<VideoPlayer>().time / ((float)VideoPlayerObject.GetComponent<VideoPlayer>().length);
    }

    IEnumerator FindCameraAndPlay()
    {
        VideoPlayerObject.GetComponent<VideoPlayer>().clip = VideoToPlay;//设置要播放的视频
        CONTROLL_TogglePlay();  //暂停视频。VideoClip必须设为自动播放，否则UNITY会特么的出BUG！！会读取不到视频文件
        while (GameObject.Find("Main Camera") == null)//这个VideoPlayer Prefab可能要比Camera先实例化出来，所以要用while去不断寻找Main Camera，直到找到为止。
        {
            //这里啥也不干，就卡着协程不让其继续执行
            print("正在寻找相机");
            yield return null;//等待一帧
        }

        //既然能脱离while循环，就说明找到Main Camera了
        CameraObject = GameObject.Find("Main Camera").GetComponent<Camera>();//为CameraObject赋值
        VideoPlayerObject.GetComponent<VideoPlayer>().targetCamera = CameraObject;//设置VideoPlayer的targetCamera
        //VideoPlayerObject.GetComponent<VideoPlayer>().url = VideoPlayerObject.GetComponent<VideoPlayer>().url.Substring(7);
        //GameManager.Instance.DoAdvPrint(VideoPlayerObject.GetComponent<VideoPlayer>().url);

        while(GameManager.Instance.isLoading == true)   //等待加载完毕
        {
            yield return null;
        }
        CONTROLL_TogglePlay();//播放视频
    
        yield break;//退出协程
    }

    /// <summary>
    /// CONTROLL_TogglePlay
    /// 作用：调用该函数会使视频 暂停/继续 播放。
    /// Toggle是“切换”的意思，即：执行一遍就停止、再执行一遍就播放。
    /// </summary>
    private bool isplaying = true;//标记播放状态的变量
    public void CONTROLL_TogglePlay()
    {
        if(CameraObject == null) print("找不到可用相机。");
        if(isplaying)
        {
            VideoPlayerObject.GetComponent<VideoPlayer>().Pause();
            StatueObject.GetComponent<Image>().sprite = Image_Play;
            gameObject.GetComponent<Animation>().Play("Anim_ShowBut");//按钮图片淡入淡出
        }
        else
        {
            VideoPlayerObject.GetComponent<VideoPlayer>().Play();
            StatueObject.GetComponent<Image>().sprite = Image_Pause;
            gameObject.GetComponent<Animation>().Play("Anim_FadeBut");
        }
        isplaying = !isplaying;//切换播放状态
    }

    /// <summary>
    /// CONTROL_BackToMenu
    /// 作用：调用该函数即可返回主菜单
    /// </summary>
    public void CONTROL_BackToMenu()
    {
        GameManager.Instance.BackToMainmenu();
        GameManager.Instance.PlayButtonSound();
        Destroy(gameObject);
    }

    int playspeedindex = 0;
    public void CONTROL_SetPlaySpeed()
    {
        switch(playspeedindex)
        {
            case 0: VideoPlayerObject.GetComponent<VideoPlayer>().playbackSpeed = 1.25f; PlayspeedText.text = "1.25x"; playspeedindex++;break;
            case 1: VideoPlayerObject.GetComponent<VideoPlayer>().playbackSpeed = 1.5f; PlayspeedText.text = "1.5x"; playspeedindex++;break;
            case 2: VideoPlayerObject.GetComponent<VideoPlayer>().playbackSpeed = 0.7f; PlayspeedText.text = "0.7x"; playspeedindex++;break;
            case 3: VideoPlayerObject.GetComponent<VideoPlayer>().playbackSpeed = 1f; PlayspeedText.text = "1.0x"; playspeedindex = 0;break;
        }
    }

    /// <summary>
    /// CONTROL_OnSliderChange
    /// 作用：进度条每次变动都会调用该函数，用于完成 “进度条大于XXX就解锁下一关” 的功能。
    /// </summary>
    private bool isUnlocked = false;
    public void CONTROL_OnSliderChange()
    {
        if (SliderObject.GetComponent<Slider>().value > TargetProcess && isUnlocked == false)
        {
            GameManager.Instance.UnlockNextScene();
            isUnlocked = !isUnlocked;
        }
    }

}
