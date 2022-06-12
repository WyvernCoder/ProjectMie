using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SRP_GameMenu : MonoBehaviour
{   

    void Start()
    {
        if(!Application.isMobilePlatform)
        {
            //交换分辨率
            Screen.SetResolution(1080,1920, false);
        }
    }
    //TODO：地图载入脚本
    public void CONTROL_JoinInLoadingLevel() => StartCoroutine(ChangeLevel());
    IEnumerator ChangeLevel()
    {
        if(!Application.isMobilePlatform)
        {
            //交换分辨率
            Screen.SetResolution(1920,1080, false);
        }
        else
        {
            //设置横屏
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        //创建载入动画
        var GO = GameManager.Instance.DoLoading();
        DontDestroyOnLoad(GO);


        //隐藏安卓状态栏
        //GameManager.statusBarState = GameManager.States.Hidden;//透明风格
        
        //切换地图
        AsyncOperation scene;
        scene = SceneManager.LoadSceneAsync("SE_Mainmenu_L");
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
