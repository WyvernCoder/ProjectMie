using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// SRP_MissionWin
/// 作用：实例化后直接进入胜利界面
/// 用法：当场实例化即可，和MissionFailed用法相似，可使用自己制作的胜利页面。
/// 具体如何自己制作胜利页面，参考文档。
/// </summary>
public class SRP_MissionWin : MonoBehaviour
{
    [Header("对象引用部分")]

    [Tooltip("包含Canvas组件的GameObject，该Canvas可以自定义，具体参考文档。")]
    public GameObject PaypageCanvas;




    [Header("用户赋值部分")]

    [Tooltip("是否没有结算界面？如果没有，Win动画播放完成后将会自动跳回主菜单。")]
    public bool NoPaypage = false;

    [Tooltip("过几秒后实例化Celebrate Prefab？Win动画一共1秒播放完毕，建议设为0.9秒。")]
    public float CelebrateTimeSinceInstantiate = 0.9f;






    private Camera usingCamera;//自动寻找相机Object
    void Awake()
    {
        PaypageCanvas.gameObject.SetActive(false);//隐藏掉结算界面
        usingCamera = FindObjectOfType<Camera>();
    }


    void Start()
    {
        if(NoPaypage) Invoke("CONTROLL_CompleteLevelAndLeave", 1f);//如果没有结算界面，则在屏幕完全黑下来后再换图
        Invoke("SetCamera",0.1f);//为结算Canvas设置操作Camera
        Invoke("StartCelebrate",CelebrateTimeSinceInstantiate);//庆祝！
    }


    private void StartCelebrate()//开始庆祝！
    {
        GameManager.Instance.DoCelebrate();
    }


    private void SetCamera()
    {
        if(usingCamera == null) print("当前关卡内没有可用的Camera！");
        else 
        {
            PaypageCanvas.GetComponent<Canvas>().worldCamera = usingCamera;
        }
    }

    /// <summary>
    /// 完成当前关卡并解锁下一关卡
    /// </summary>
    public void CONTROLL_CompleteLevelAndLeave()
    {
        GameManager.Instance.PlayButtonSound();
        GameManager.Instance.BackToMainmenu();
        //TODO解锁下一关
        Destroy(gameObject);
    }
}
