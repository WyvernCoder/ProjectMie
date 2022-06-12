using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SRP_Control : MonoBehaviour
{
    [Header("对象赋值部分")]

    [Tooltip("返回按钮的引用。")]
    public Button ReturnButton;

    [HideInInspector]
    public GameObject CurrentSubMenu;

    [HideInInspector]
    public Vector3 CameraLocation;//记录开始游戏前相机的位置

    [HideInInspector]
    public SRP_TouchMover TouchMover;

    private GameObject IslandCollection;

    void Start()
    {
        TouchMover = GameObject.Find("PRB_TouchMover").gameObject.GetComponent<SRP_TouchMover>();
        IslandCollection = GameObject.Find("IslandCollection").gameObject;
    }

    public void CONTROL_ReturnToMainmenu()
    {
        if(CurrentSubMenu == null)//如果没有子目录Object就返回主菜单
        StartCoroutine(ChangeLevel());
        else 
        {   //检测子目录是否被关闭
            if(CurrentSubMenu.GetComponent<Canvas>().enabled == true) 
            {
                CurrentSubMenu.GetComponent<Canvas>().enabled = false;
                TouchMover.CanMoveCamera = true;//使相机可以继续被移动
                CurrentSubMenu = null;//初始化子目录Object引用
            }
            else StartCoroutine(ChangeLevel());//如果子目录早已经关闭，就返回主菜单
        }
    }

    IEnumerator ChangeLevel()
    {
        //创建载入动画
        var GO = GameManager.Instance.DoLoading();
        DontDestroyOnLoad(GO);

        //设置横屏
        Screen.orientation = ScreenOrientation.Portrait;
        
        //切换地图
        AsyncOperation scene;
        scene = SceneManager.LoadSceneAsync("SE_Mainmenu_P");
        scene.allowSceneActivation = false;

        //等待地图载入完成
        while (scene.progress <= 0.89f)
        {
            //正在载入
            yield return null;
        }

        //触发载入动画结束阶段
        GO.GetComponent<SRP_LoadingAnim>().CloseTheLoad(2);

        //切换地图
        scene.allowSceneActivation = true;
    }
}
